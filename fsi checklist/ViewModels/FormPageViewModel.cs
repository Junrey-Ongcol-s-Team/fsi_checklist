// =====================================================================
// PAGE 3 (Form) - ViewModel.
//   - i-load ang form gikan sa database ug himoon ang mga row (section + items)
//   - autosave: matag usab, maghulat 0.6 segundo dayon i-save
//   - progress bar, 'Next missing', 'Save draft', ug 'Sign-off'
// =====================================================================
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fsi_checklist.Data.Models;
using fsi_checklist.Services;
using fsi_checklist.Views;

namespace fsi_checklist.ViewModels;

[QueryProperty(nameof(InspectionId), "inspectionId")]
public sealed partial class FormPageViewModel : ObservableObject
{
    private readonly InspectionService _inspections;
    private readonly SemaphoreSlim _saveLock = new(1, 1);
    private readonly HashSet<SlotViewModel> _dirty = new();
    private readonly Dictionary<string, SlotViewModel> _valueSlots = new();
    private CancellationTokenSource? _debounce;
    private readonly List<SlotViewModel> _allSlots = new();
    private IReadOnlyCollection<FormItem> _items = Array.Empty<FormItem>();
    private Dictionary<string, List<FormColumn>> _columnsBySet = new();
    private bool _highlightMissing;

    // Tanang row (flat, apil ang sulod sa expander). Gamiton sa visibility, progress ug Next missing.
    // Ang 'Rows' kay para sa screen ra: ang sulod sa expander anaa sa ItemRowViewModel.Children.
    private List<FormRowViewModel> _flatRows = new();

    // Mga pangutana nga gigamit sa show_if (pananglitan 'Vehicle type' sa LPG).
    // Kung kini ra ang mausab, dayon i-refresh ang visibility. Ang uban dili na kinahanglan.
    private HashSet<string> _triggerItems = new();

    public event Action<int>? ScrollToRowRequested;

    public FormPageViewModel(InspectionService inspections)
    {
        _inspections = inspections;
    }

    [ObservableProperty]
    private string inspectionId = string.Empty;

    [ObservableProperty]
    private string title = "Inspection";

    [ObservableProperty]
    private string subtitle = string.Empty;

    [ObservableProperty]
    private string saveStatus = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private List<FormRowViewModel> rows = new();

    [ObservableProperty]
    private double progressFraction;

    [ObservableProperty]
    private string progressText = string.Empty;

    [ObservableProperty]
    private Color progressColor = Color.FromArgb("#F9A825");

    [ObservableProperty]
    private bool isFormComplete;

    partial void OnInspectionIdChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
            _ = LoadAsync();
    }

    // ----- loading -----------------------------------------------------------

    // I-load ang form ug ang mga tubag nga naa na (mao ni ang pagbalik sa draft).
    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var data = await _inspections.LoadFormAsync(InspectionId);
            if (data == null)
            {
                await Dialogs.ShowAsync("Not found", "This inspection no longer exists.");
                await Nav.GoAsync("..");
                return;
            }

            Title = data.Inspection.Template.DisplayName;
            Subtitle = BuildSubtitle(data.Inspection);
            Rows = BuildRows(data);
            _items = data.Items;
            _columnsBySet = data.Columns
                .GroupBy(c => c.ColumnSetId)
                .ToDictionary(g => g.Key, g => g.ToList());
            _triggerItems = data.Items.Select(i => i.ShowIfItemId)
                .Concat(data.Sections.Select(x => x.ShowIfItemId))
                .Where(id => !string.IsNullOrEmpty(id))
                .Select(id => id!)
                .ToHashSet();
            foreach (var id in _allSlots.Where(s => s.SlotKey == "result").Select(s => s.ItemId).Distinct().ToList())
                ApplyNaRule(id, clear: false);
            RefreshVisibility();
            RecomputeProgress();
        }
        catch (Exception ex)
        {
            AppLog.Error(ex);
            await Dialogs.ShowAsync("Couldn't open inspection", AppLog.Describe(ex));
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static string BuildSubtitle(Inspection inspection)
    {
        var parts = new List<string> { inspection.Subject.Name };
        if (!string.IsNullOrWhiteSpace(inspection.ReferenceNo))
            parts.Add($"Ref. {inspection.ReferenceNo}");
        if (inspection.DateInspected is { } date)
            parts.Add(date.ToString("d MMM yyyy"));
        return string.Join(" · ", parts);
    }

    // Himoon ang mga row sa screen: seksyon, dayon ang mga pangutana sulod niini (ug ang mga anak nga pangutana).
    private List<FormRowViewModel> BuildRows(FormData data)
    {
        _valueSlots.Clear();
        _dirty.Clear();
        _allSlots.Clear();

        var answers = data.Answers
            .Where(a => a.RepeatNo == 0)
            .ToDictionary(a => (a.ItemId, a.SlotKey));

        var itemChoices = data.Choices
            .Where(c => c.ItemId != null)
            .GroupBy(c => c.ItemId!)
            .ToDictionary(g => g.Key, g => g.OrderBy(c => c.SortOrder).ToList());

        var columnChoices = data.Choices
            .Where(c => c.ColumnId != null)
            .GroupBy(c => c.ColumnId!)
            .ToDictionary(g => g.Key, g => g.OrderBy(c => c.SortOrder).ToList());

        var columnsBySet = data.Columns
            .GroupBy(c => c.ColumnSetId)
            .ToDictionary(g => g.Key, g => g.OrderBy(c => c.SortOrder).ToList());

        var rowsOut = new List<FormRowViewModel>();

        void AddItem(FormItem item, int depth)
        {
            var slots = new List<SlotViewModel>();

            columnsBySet.TryGetValue(item.ColumnSetId ?? string.Empty, out var columns);
            int slotCount = (item.DataType != "NONE" ? 1 : 0) + (columns?.Count ?? 0);
            bool labelled = slotCount > 1;

            if (item.DataType != "NONE")
            {
                itemChoices.TryGetValue(item.Id, out var choices);
                slots.Add(MakeSlot(item.Id, "value", null, item.DataType, choices, item.Unit));
            }

            if (columns != null)
            {
                foreach (var column in columns)
                {
                    columnChoices.TryGetValue(column.Id, out var choices);
                    slots.Add(MakeSlot(item.Id, column.Key, labelled ? column.Label : null,
                        column.DataType, choices, null));
                }
            }

            // N/A button sa numero sa Fire Drill / Fire Insurance (ang date issued mawala kung N/A, tan-awa na_text.sql)
            if (NaTextKeys.Contains(item.Key) && slots.Count > 0)
                slots[0].CanNa = true;

            foreach (var slot in slots)
            {
                if (answers.TryGetValue((slot.ItemId, slot.SlotKey), out var answer))
                    slot.LoadFrom(answer.ValueText, answer.ValueNum);

                slot.Changed += OnSlotChanged;
                _allSlots.Add(slot);

                if (slot.SlotKey == "value")
                    _valueSlots[item.Id] = slot;
            }

            // EXPANDER: (1) ang numero sa ExpanderSections nga naay sulod (anak), o
            //           (2) ang pangutana sa ExpanderItems (ang iyang pilian mismo ang naa sa sulod).
            bool expandable = item.ParentItemId == null
                && ((ExpanderSections.Contains(item.SectionId) && data.Items.Any(i => i.ParentItemId == item.Id))
                    || ExpanderItems.Contains(item.Id)
                    || ExpanderEveryNumber.Contains(item.SectionId));
            var row = new ItemRowViewModel(item, slots, depth, expandable);
            if (expandable)
                row.Collapsed += r => ScrollToRowRequested?.Invoke(Rows.IndexOf(r));   // Done: balik sa numero

            // LITRATO: ang PHOTO nga pangutana (064) ug ang mga item sa PhotoItems (ubos) makadawat og litrato.
            if (item.DataType == "PHOTO")
            {
                itemChoices.TryGetValue(item.Id, out var photoChoices);
                if (photoChoices is { Count: > 0 })
                    foreach (var c in photoChoices)
                        row.PhotoSlots.Add(new PhotoSlotViewModel(item.Id, c.Value, c.Label));
                else
                    row.PhotoSlots.Add(new PhotoSlotViewModel(item.Id, null, "Photos"));
            }
            else if (PhotoItems.TryGetValue(item.Key, out var photoLabel))
            {
                row.PhotoSlots.Add(new PhotoSlotViewModel(item.Id, null, photoLabel));
            }

            foreach (var ps in row.PhotoSlots)
            {
                foreach (var a in data.Photos.Where(p => p.ItemId == ps.ItemId && p.Tag == ps.Tag))
                    ps.Photos.Add(new PhotoViewModel(ps, a.Id, InspectionService.PhotoFullPath(a.FilePath)));
            }

            rowsOut.Add(row);

            foreach (var child in data.Items
                         .Where(i => i.ParentItemId == item.Id)
                         .OrderBy(i => i.SortOrder))
            {
                AddItem(child, depth + 1);
            }
        }

        // ULOHAN NGA KAUSA RA: ang mga seksyon nga "Fire protection: A. ...", "Fire protection: B. ..." nga
        // usa ra ka ExpanderView ang sulod -> "Fire protection" kausa ra sa ibabaw; ang A, B ... kay ang mga expander.
        string? lastGroup = null;

        void AddSection(FormSection section)
        {
            var tops = data.Items.Where(i => i.SectionId == section.Id && i.ParentItemId == null).ToList();
            int colon = section.Title.IndexOf(": ", StringComparison.Ordinal);
            bool grouped = colon > 0 && tops.Count == 1 && ExpanderItems.Contains(tops[0].Id);
            if (grouped)
            {
                string group = section.Title[..colon];
                if (group != lastGroup)
                    rowsOut.Add(new SectionRowViewModel(section, group));
                lastGroup = group;
            }
            else if (tops.Count == 1 && ExpanderItems.Contains(tops[0].Id) && tops[0].Label == section.Title)
            {
                // Pareho ang ngalan sa seksyon ug sa expander (068 "General information"): ang expander ra, dili doble
                lastGroup = null;
            }
            else
            {
                rowsOut.Add(new SectionRowViewModel(section));
                lastGroup = null;
            }

            foreach (var item in data.Items
                         .Where(i => i.SectionId == section.Id && i.ParentItemId == null)
                         .OrderBy(i => i.SortOrder))
            {
                AddItem(item, 0);
            }

            foreach (var child in data.Sections
                         .Where(s => s.ParentId == section.Id)
                         .OrderBy(s => s.SortOrder))
            {
                AddSection(child);
            }
        }

        foreach (var root in data.Sections.Where(s => s.ParentId == null).OrderBy(s => s.SortOrder))
            AddSection(root);

        _flatRows = rowsOut;

        // EXPANDER: ibalhin ang mga pangutana sulod sa numero ngadto sa iyang Children (body sa ExpanderView).
        // Ang mga anak sunod-sunod human sa numero, hangtod sa sunod nga numero o seksyon.
        var display = new List<FormRowViewModel>();
        ItemRowViewModel? header = null;
        foreach (var r in rowsOut)
        {
            if (r is ItemRowViewModel ir)
            {
                if (ir.IsExpandable)
                {
                    header = ir;
                    display.Add(ir);
                    continue;
                }
                if (header != null && ir.Item.ParentItemId != null)
                {
                    header.Children.Add(ir);
                    continue;
                }
            }
            header = null;
            display.Add(r);
        }

        return display;
    }

    private static SlotViewModel MakeSlot(
        string itemId, string slotKey, string? label, string dataType,
        List<FormChoice>? choices, string? unit)
    {
        static IEnumerable<(string, string)> Fixed(params (string, string)[] values) => values;

        switch (dataType)
        {
            case "YES_NO":
                return new SlotViewModel(itemId, slotKey, label, SlotKind.Choice,
                    Fixed(("YES", "Yes"), ("NO", "No")));

            case "YES_NO_NA":
                return new SlotViewModel(itemId, slotKey, label, SlotKind.Choice,
                    Fixed(("YES", "Yes"), ("NO", "No"), ("NA", "N/A")));

            case "PASS_FAIL":
                return new SlotViewModel(itemId, slotKey, label, SlotKind.Choice,
                    Fixed(("PASS", "Passed"), ("FAIL", "Failed")));

            case "CHOICE":
                // PICKER (dropdown) para sa taas nga listahan (tan-awa PickerItems)
                return new SlotViewModel(itemId, slotKey, label,
                    PickerItems.Contains(itemId) ? SlotKind.Picker : SlotKind.Choice,
                    (choices ?? new()).Select(c => (c.Value, c.Label)));

            case "BOOL":
                return new SlotViewModel(itemId, slotKey, label, SlotKind.Bool);

            case "DATE":
                return new SlotViewModel(itemId, slotKey, label, SlotKind.Date);

            case "NUMBER":
                return new SlotViewModel(itemId, slotKey, label, SlotKind.Number, hint: unit);

            case "INTEGER":
                return new SlotViewModel(itemId, slotKey, label, SlotKind.Number, hint: unit, isInteger: true);

            case "LONG_TEXT":
                // CHIPS: daghan nga mapili + pwede mag-type og lain (tan-awa ChipItems)
                if (ChipItems.Contains(itemId))
                    return new SlotViewModel(itemId, slotKey, label, SlotKind.Chips,
                        (choices ?? new()).Select(c => (c.Value, c.Label)));
                return new SlotViewModel(itemId, slotKey, label, SlotKind.LongText);

            case "PHOTO":
                string slots = choices is { Count: > 0 }
                    ? string.Join(", ", choices.Select(c => c.Label))
                    : "photos";
                return new SlotViewModel(itemId, slotKey, label, SlotKind.Photo,
                    hint: $"Photos required: {slots}.");

            case "DATETIME":
                return new SlotViewModel(itemId, slotKey, label, SlotKind.Text, hint: "yyyy-MM-dd HH:mm");

            default: // TEXT and anything unknown
                return new SlotViewModel(itemId, slotKey, label, SlotKind.Text);
        }
    }

    // ----- visibility (show_if rules) -----------------------------------------

    private bool Evaluate(string? itemId, string? values)
    {
        if (string.IsNullOrEmpty(itemId))
            return true;

        // Mahimo nga value o column (pananglitan "change_status:WITH_CHANGES") ang tan-awon.
        var (slotKey, list) = ShowIfRule.Parse(values);
        var slot = slotKey == "value"
            ? _valueSlots.GetValueOrDefault(itemId)
            : _allSlots.FirstOrDefault(s => s.ItemId == itemId && s.SlotKey == slotKey);
        return ShowIfRule.Matches(slot?.GetStored().Text, list);
    }

    // Itago o ipakita ang mga pangutana base sa show_if (pananglitan Two-wheeled / Three-wheeled).
    private void RefreshVisibility()
    {
        var sectionVisible = new Dictionary<string, bool>();
        foreach (var row in _flatRows.OfType<SectionRowViewModel>())
        {
            var s = row.Section;
            bool visible = Evaluate(s.ShowIfItemId, s.ShowIfValues)
                && (s.ParentId == null || sectionVisible.GetValueOrDefault(s.ParentId, true));
            sectionVisible[s.Id] = visible;
            row.IsVisible = visible;
        }

        var itemVisible = new Dictionary<string, bool>();
        foreach (var row in _flatRows.OfType<ItemRowViewModel>())
        {
            var item = row.Item;
            bool visible = Evaluate(item.ShowIfItemId, item.ShowIfValues)
                && sectionVisible.GetValueOrDefault(item.SectionId, true)
                && (item.ParentItemId == null || itemVisible.GetValueOrDefault(item.ParentItemId, true));
            itemVisible[item.Id] = visible;
            row.IsVisible = visible;
        }
    }

    // ----- saving ---------------------------------------------------------------

    // Matag usab sa tubag: markahan nga 'dirty', i-update ang visibility ug progress, ug i-save pagkahuman.
    // PARA DILI MO-LAG:
    //   - Pag-tap (Yes/No, checkbox, petsa): i-update dayon ang screen.
    //   - Pag-type (text/numero): DILI i-update matag letra. I-update ra human sa 0.6 segundo nga hunong (sa SaveSoonAsync).
    private void OnSlotChanged(SlotViewModel slot)
    {
        _dirty.Add(slot);
        SaveStatus = "Editing…";

        bool isTyping = slot.Kind is SlotKind.Text or SlotKind.LongText or SlotKind.Number;

        // N/A: kung "N/A" ang gipili sa Passed / Failed / N/A, itago ug limpyohan ang numero (dili na kinahanglan).
        if (slot.SlotKey == "result" && slot.Kind == SlotKind.Choice)
            ApplyNaRule(slot.ItemId, clear: true);

        if (!isTyping)
            UpdateScreen(slot.ItemId);

        _ = SaveSoonAsync();
    }

    // Giihap ba kini nga slot sa progress? Pareho sa ProgressCalculator:
    // value (kung pilian o required), mga pilian nga column, ug ang Remarks kung FAILED.
    private static bool SlotCounts(ItemRowViewModel row, SlotViewModel slot)
    {
        if (slot.SlotKey == "value")
            return ProgressCalculator.ValueSlotCounts(row.Item);
        if (slot.Kind == SlotKind.Choice)
            return true;
        var result = row.Slots.FirstOrDefault(s => s.SlotKey == "result");
        return ProgressCalculator.RemarksRequired(slot.SlotKey, result?.GetStored().Text);
    }

    // N/A NGA LAGDA: ang numero (pananglitan "___ m") sa pangutana nga naay Passed / Failed / N/A
    // matago kung N/A ang gipili. Kung clear = true, papason usab ang numero aron dili ma-print.
    private void ApplyNaRule(string itemId, bool clear)
    {
        var result = _allSlots.FirstOrDefault(s => s.ItemId == itemId && s.SlotKey == "result" && s.Kind == SlotKind.Choice);
        var number = _allSlots.FirstOrDefault(s => s.ItemId == itemId && s.SlotKey == "value" && s.Kind == SlotKind.Number);
        if (result == null || number == null)
            return;

        bool na = string.Equals(result.GetStored().Text, "NA", StringComparison.OrdinalIgnoreCase);
        number.IsHidden = na;
        if (na && clear && !string.IsNullOrEmpty(number.Text))
            number.Text = null;
    }

    // I-refresh ang visibility (kung kinahanglan ra) ug ang progress bar.
    private void UpdateScreen(string? changedItemId)
    {
        if (changedItemId == null || _triggerItems.Contains(changedItemId))
            RefreshVisibility();
        RecomputeProgress();
    }

    // AUTOSAVE: maghulat og 0.6 segundo. Kung mag-type pa, i-restart ang paghulat.
    private async Task SaveSoonAsync()
    {
        _debounce?.Cancel();
        _debounce = new CancellationTokenSource();
        var token = _debounce.Token;

        try
        {
            await Task.Delay(600, token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        // Human mohunong og type: i-update ang screen kausa, dayon i-save.
        bool typedTrigger = _dirty.Any(d => _triggerItems.Contains(d.ItemId));
        UpdateScreen(typedTrigger ? null : string.Empty);

        await FlushAsync();
    }

    // I-save dayon ang tanang wala pa na-save nga tubag.
    /// <summary>Writes pending edits immediately (also called when the page goes away).</summary>
    public async Task FlushAsync()
    {
        _debounce?.Cancel();

        await _saveLock.WaitAsync();
        try
        {
            if (_dirty.Count == 0 || string.IsNullOrEmpty(InspectionId))
                return;

            var batch = _dirty.ToList();
            _dirty.Clear();

            var writes = batch
                .Select(s =>
                {
                    var (text, num) = s.GetStored();
                    return new AnswerWrite(s.ItemId, s.SlotKey, text, num);
                })
                .ToList();

            try
            {
                SaveStatus = "Saving…";
                await _inspections.SaveAnswersAsync(InspectionId, writes);
                SaveStatus = _dirty.Count == 0 ? "All changes saved" : "Editing…";
            }
            catch (Exception ex)
            {
                AppLog.Error(ex);
                foreach (var slot in batch)
                    _dirty.Add(slot);
                SaveStatus = "Couldn't save changes";
            }
        }
        finally
        {
            _saveLock.Release();
        }
    }

    // ----- progress, draft and sign-off -------------------------------------------

    // Kuwentahon ang progress bar gikan sa mga tubag nga naa sa screen karon.
    private void RecomputeProgress()
    {
        var answers = new Dictionary<(string, string), string>();
        foreach (var slot in _allSlots)
        {
            var (text, num) = slot.GetStored();
            if (text != null || num != null)
                answers[(slot.ItemId, slot.SlotKey)] = text ?? string.Empty;
        }

        var result = ProgressCalculator.Compute(_items, _columnsBySet, answers);

        ProgressFraction = result.Fraction;
        IsFormComplete = result.IsComplete;
        ProgressColor = result.IsComplete ? Color.FromArgb("#2E7D32") : Color.FromArgb("#F9A825");
        ProgressText = result.IsComplete
            ? $"All {result.Total} items done"
            : $"{result.Missing} of {result.Total} items still missing";

        var rows = _flatRows.OfType<ItemRowViewModel>().ToList();
        foreach (var row in rows)
            row.IsMissing = _highlightMissing && row.IsVisible && IsRowMissing(row, answers);

        // CIRCLE PROGRESS: ihapon ang natubag sulod sa matag numero (ang makita ra, pareho sa progress bar).
        foreach (var header in rows.Where(r => r.IsExpandable))
        {
            int total = 0, done = 0;
            // Apil ang kaugalingong pilian sa header (pananglitan Construction type) ug ang mga anak niini.
            foreach (var child in header.Children.Where(c => c.IsVisible).Prepend(header))
            {
                foreach (var slot in child.Slots)
                {
                    bool counts = SlotCounts(child, slot);
                    if (!counts)
                        continue;
                    total++;
                    if (answers.ContainsKey((slot.ItemId, slot.SlotKey)))
                        done++;
                }
            }

            bool complete = total > 0 && done == total;
            header.HasProgress = total > 0 && !NoCircleProgress.Contains(header.Item.Id);
            header.Progress = total == 0 ? 0 : (double)done / total;
            header.ProgressText = complete ? "✓" : $"{done}/{total}";
            header.RingColor = complete ? Color.FromArgb("#2E7D32") : Color.FromArgb("#F9A825");
        }

        // EXPANDER: yellow usab ang numero kung naay kulang sa sulod niini (bisan nakasira).
        foreach (var header in rows.Where(r => r.IsExpandable && r.IsVisible))
        {
            if (!header.IsMissing)
                header.IsMissing = rows.Any(r => r.IsMissing && !r.IsExpandable && IsInside(r.Item, header.Item.Id));
        }
    }

    // Sulod ba ang item sa numero (parent, o parent sa parent...)?
    private bool IsInside(FormItem item, string headerId)
    {
        var byId = _items.ToDictionary(i => i.Id);
        string? parent = item.ParentItemId;
        for (int depth = 0; parent != null && depth < 20; depth++)
        {
            if (parent == headerId)
                return true;
            parent = byId.TryGetValue(parent, out var up) ? up.ParentItemId : null;
        }
        return false;
    }

    private static bool IsRowMissing(ItemRowViewModel row, IReadOnlyDictionary<(string, string), string> answers)
    {
        foreach (var slot in row.Slots)
        {
            bool counts = SlotCounts(row, slot);

            if (counts && !answers.ContainsKey((slot.ItemId, slot.SlotKey)))
                return true;
        }

        return false;
    }

    // SAVE DRAFT: i-save ang katapusang tubag ug mobalik sa Dashboard. Mabalikan kini bisan kanus-a.
    /// <summary>Answers are saved as you type; this just makes sure the last edit is written, then goes back.</summary>
    [RelayCommand]
    private async Task SaveDraftAsync()
    {
        await FlushAsync();
        await Nav.GoAsync("..");
    }

    // NEXT MISSING: i-frame og yellow ang kulang nga mga item ug mo-scroll sa una.
    [RelayCommand]
    private async Task NextMissingAsync()
    {
        await FlushAsync();

        _highlightMissing = true;
        RecomputeProgress();

        // Ang tinuod nga kulang nga pangutana (dili ang numero nga expander).
        var target = _flatRows.OfType<ItemRowViewModel>().FirstOrDefault(r => r.IsMissing && !r.IsExpandable)
            ?? _flatRows.OfType<ItemRowViewModel>().FirstOrDefault(r => r.IsMissing);
        if (target == null)
        {
            await Dialogs.ShowAsync("Nothing missing", "Every required item is filled in.");
            return;
        }

        ShowRow(target);
    }

    // Ipakita ang pangutana: kung sulod sa expander, ablihan kini ug mo-scroll sa numero.
    private void ShowRow(ItemRowViewModel target)
    {
        var header = Rows.OfType<ItemRowViewModel>()
            .FirstOrDefault(r => r.IsExpandable && (r == target || r.Children.Contains(target)));
        if (header != null)
        {
            header.IsExpanded = true;
            target = header;
        }

        ScrollToRowRequested?.Invoke(Rows.IndexOf(target));
    }

    // SIGN-OFF: mugana ra kung humana na ang tanang kinahanglan nga tubag.
    [RelayCommand]
    private async Task GoToSignoffAsync()
    {
        await FlushAsync();

        if (!IsFormComplete)
        {
            await Dialogs.ShowAsync(
                "The form isn't finished yet",
                $"{ProgressText}. Tap “Next missing” to find them, or tap “Save draft” and come back later.");
            return;
        }

        // ALERT SA PETSA (tanang forms): kung naay petsa nga wala pa mabutang ("Set date" pa),
        // ipakita ang listahan. "Fill in dates" = adto sa una nga walay petsa; "Continue anyway" = padayon.
        var noDate = _flatRows.OfType<ItemRowViewModel>()
            .Where(r => r.IsVisible)
            .SelectMany(r => r.Slots
                .Where(sl => sl.Kind == SlotKind.Date && !sl.HasDate && !sl.IsHidden)
                .Select(sl => (Row: r, Name: string.IsNullOrEmpty(sl.Label) ? r.Item.Label : $"{r.Item.Label} ({sl.Label})")))
            .ToList();
        if (noDate.Count > 0)
        {
            var names = noDate.Take(8).Select(x => "\u2022 " + x.Name).ToList();
            if (noDate.Count > 8)
                names.Add($"\u2022 ... and {noDate.Count - 8} more");
            bool fill = await Dialogs.ConfirmAsync(
                "No date yet",
                $"{noDate.Count} date field(s) are still empty:\n\n{string.Join("\n", names)}",
                "Fill in dates", "Continue anyway");
            if (fill)
            {
                ShowRow(noDate[0].Row);
                return;
            }
        }

        await Nav.GoAsync(
            $"{nameof(SignoffPage)}?inspectionId={Uri.EscapeDataString(InspectionId)}&fromForm=1");
    }

    // ----- litrato -----------------------------------------------------------------

    // TAKE PHOTO (camera) o CHOOSE PHOTO (gallery / file). Tawgon gikan sa FormPage.xaml.cs.
    // =================================================================
    // LISTAHAN SA MGA ITEM NGA MAKA-ATTACH OG LITRATO (item key -> label sa button).
    // Gusto og dugang? Idugang lang dinhi ang key sa item (tan-awa ang form_item.key sa database).
    //   defects          = tanang form nga naay "Defects / deficiencies" (021, 024, 026, 051, 061, 068, 069, 075)
    //   test.01 - 03     = 075 Pipelines: "Attach hardcopy of all test records"
    //   nod_deficiencies = 006 Building plans: kakulangan sa plano
    // (Ang 064 LPG motorcycle naa nay kaugalingon nga PHOTO item: FRONT / REAR / SIDE.)
    // =================================================================
    // =================================================================
    // EXPANDER: mga seksyon nga ang matag numero kay expander (sirado ang sulod hangtod i-tap).
    //   fsic-renewal.s.assessment = 069 FSIC renewal, V. Fire Safety Measures Assessment (1 hangtod 13)
    // Aron mahimo usab sa laing form: i-add diri ang section id.
    // =================================================================
    private static readonly HashSet<string> ExpanderSections = new()
    {
        "fsic-renewal.s.assessment",
    };

    // EXPANDER ANG MATAG NUMERO (bisan walay anak): ang sulod = ang iyang tubag (Yes/No/N/A, numero ...).
    //   068 Inspection during construction: I. hangtod VII. (tanang numero 1 hangtod 95)
    private static readonly HashSet<string> ExpanderEveryNumber = new()
    {
        "during-construction.s.egress",         // 068 I. Means of egress
        "during-construction.s.construction",   // 068 II.A Construction and compartmentation
        "during-construction.s.fire_doors",     // 068 II.B Fire doors
        "during-construction.s.vertical",       // 068 II.C Protection of vertical opening
        "during-construction.s.warnings",       // 068 III. Warnings (signs, lighting, exit signage)
        "during-construction.s.suppression",    // 068 IV.A Automatic fire suppression system
        "during-construction.s.standpipe",      // 068 IV.B Wet standpipe system
        "during-construction.s.detection",      // 068 IV.C Fire detection and alarm system
        "during-construction.s.extinguishers",  // 068 IV.D Portable fire extinguisher
        "during-construction.s.services",       // 068 V. Building service equipment
        "during-construction.s.other",          // 068 VI. Other fire safety requirements
        "during-construction.s.precautions",    // 068 VII. Fire safety precautions
    };

    // Mga expander nga WALAY circle progress (pananglitan 061 Means of egress). ExpanderView ug Done ra.
    private static readonly HashSet<string> NoCircleProgress = new()
    {
        // (walay sulod karon: gibalik ang circle sa Means of egress ug Signs)
    };

    // EXPANDER sa usa ka pangutana: ang header = ngalan sa pangutana, ang sulod = ang iyang pilian
    // (ug ang iyang mga anak, pananglitan "Others (specify)"). Sirado sa sugod.
    private static readonly HashSet<string> ExpanderItems = new()
    {
        "fsic-new-occupancy.nature",               // 061 2. Nature of inspection conducted
        "fsic-new-occupancy.req.fsic_for",         // 061 3. Requirements: FSIC for Occupancy / New Business Permit
        "fsic-new-occupancy.fsic_for",             // 061 4. General information: FSIC for Occupancy / New Business Permit
        "fsic-new-occupancy.occupancy_class",      // 061 General occupancy classification
        "fsic-new-occupancy.construction_type",    // 061 Construction type
        "fsic-new-occupancy.wall_finish",          // 061 Walls / ceiling interior finish
        "fsic-new-occupancy.floor_finish",         // 061 Floor interior finish
        "fsic-new-occupancy.sectional",            // 061 Sectional occupancy (Basement ... Nth Floor)
        "fsic-new-occupancy.egress.a",             // 061 Means of egress: A. Exit access
        "fsic-new-occupancy.egress.b",             // 061 Means of egress: B. Exits
        "fsic-new-occupancy.egress.c",             // 061 Means of egress: C. Exits discharge
        "fsic-new-occupancy.signs.a",              // 061 Signs: A. Marking of means of egress (EXIT)
        "fsic-new-occupancy.signs.b",              // 061 Signs: B. Emergency evacuation plan
        "fsic-new-occupancy.signs.c",              // 061 Signs: C. Illumination of means of egress
        "fsic-new-occupancy.hazard.grp_a",         // 061 Hazard: A. Other flammable liquids
        "fsic-new-occupancy.hazard.grp_b",         // 061 Hazard: B. Miscellaneous hazards
        "fsic-new-occupancy.hazard.grp_c",         // 061 Hazard: C. Housekeeping, maintenance, storage & waste disposal
        "fsic-new-occupancy.fp_sprinkler.grp",     // 061 Fire protection: A. Sprinkler
        "fsic-new-occupancy.fp_standpipe.grp",     // 061 Fire protection: B. Wet standpipe / fire hose cabinet
        "fsic-new-occupancy.fp_pump.grp",          // 061 Fire protection: C. Fire pump
        "fsic-new-occupancy.fp_detection.grp",     // 061 Fire protection: D. Fire detection system
        "fsic-new-occupancy.fp_alarm.grp",         // 061 Fire protection: E. Fire alarm facilities
        "fsic-new-occupancy.fp_lifts.grp",         // 061 Fire protection: F. Lifts (elevator)
        "fsic-new-occupancy.fp_extinguishers.grp", // 061 Fire protection: G. Fire extinguishers
        "fsic-new-occupancy.fp_emergency.grp",     // 061 Fire protection: H. Emergency lighting
        "fsic-new-occupancy.fp_kitchen.grp",       // 061 Fire protection: I. Kitchen
        "fsic-new-occupancy.fp_utilities.grp",     // 061 Fire protection: J.1 Utilities
        "fsic-new-occupancy.fp_hvac.grp",          // 061 Fire protection: J.2 HVAC
        "fsic-new-occupancy.fp_smoke.grp",         // 061 Fire protection: J.3 Smoke control
        "fsic-new-occupancy.fp_chutes.grp",        // 061 Fire protection: J.4 Chutes / incinerators
        "fsic-new-occupancy.fp_firewall.grp",      // 061 Fire protection: K. Fire wall
        "during-construction.general",             // 068 General information (Stage of inspection, FSEC ...)
    };

    // N/A BUTTON: mga TEXT nga pangutana nga naay "N/A" tapad sa sulatanan (tanang forms nga naa niini).
    private static readonly HashSet<string> NaTextKeys = new()
    {
        "fire_drill_no",         // Certificate of Fire Drill
        "insurance_policy_no",   // Fire Insurance Policy No. (if any)
    };

    // PICKER: mga CHOICE nga dropdown ang hitsura (dili chips), para sa taas nga listahan.
    private static readonly HashSet<string> PickerItems = new()
    {
        "fsic-renewal.a03.prev_occupancy",   // 069 3. General Occupancy: Previous type of occupancy
    };

    // CHIPS: mga LONG_TEXT nga pangutana nga chips ang hitsura (ang andam nga chips = form_choice sa item).
    //   Ang tubag = matag chip sa iyang linya (matag chip = usa ka linya sa print).
    private static readonly HashSet<string> ChipItems = new()
    {
        "fsic-new-occupancy.rec.clearances",   // 061 Secure the following Fire Safety Clearances
        "fsic-renewal.rec.clearances",         // 069 Secure the following Fire Safety Clearance/s
    };

    private static readonly Dictionary<string, string> PhotoItems = new()
    {
        ["defects"] = "Photos (pictures, sketch)",
        ["test.01"] = "Photo of the test record",
        ["test.02"] = "Photo of the test record",
        ["test.03"] = "Photo of the test record",
        ["nod_deficiencies"] = "Photos of the plan / deficiencies",
    };

    // LITRATO SA CELLPHONE: gamyan ang litrato (max 1920 px, quality 80) aron dili bug-at ang phone ug database.
    // RotateImage = husto ang tuyok (dili patagilid). Quality ubos sa 90 = JPEG gihapon (dili PNG).
    private static readonly MediaPickerOptions PhotoOptions = new()
    {
        MaximumWidth = 1920,
        MaximumHeight = 1920,
        CompressionQuality = 80,
        RotateImage = true,
    };

    public async Task AddPhotoAsync(PhotoSlotViewModel slot, bool useCamera)
    {
        try
        {
            FileResult? photo;
            if (useCamera)
            {
                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    await Dialogs.ShowAsync("No camera", "This device has no camera. Use \u201cChoose photo\u201d instead.");
                    return;
                }
                photo = await MediaPicker.Default.CapturePhotoAsync(PhotoOptions);
            }
            else
            {
                photo = await MediaPicker.Default.PickPhotoAsync(PhotoOptions);
            }

            if (photo == null)
                return;   // gi-cancel sa user

            var row = await _inspections.AddPhotoAsync(InspectionId, slot.ItemId, slot.Tag, photo);
            slot.Photos.Add(new PhotoViewModel(slot, row.Id, InspectionService.PhotoFullPath(row.FilePath)));
            SaveStatus = "Photo saved";
        }
        catch (Exception ex)
        {
            AppLog.Error(ex, "Add photo");
            await Dialogs.ShowAsync("Couldn't add the photo", AppLog.Describe(ex));
        }
    }

    // REMOVE: pangutan-on una, dayon tangtangon ang litrato.
    public async Task RemovePhotoAsync(PhotoViewModel photo)
    {
        if (!await Dialogs.ConfirmAsync("Remove photo?", "This photo will be deleted.", "Remove"))
            return;

        try
        {
            await _inspections.RemovePhotoAsync(photo.Id);
            photo.Slot.Photos.Remove(photo);
            SaveStatus = "Photo removed";
        }
        catch (Exception ex)
        {
            AppLog.Error(ex, "Remove photo");
            await Dialogs.ShowAsync("Couldn't remove the photo", AppLog.Describe(ex));
        }
    }
}
