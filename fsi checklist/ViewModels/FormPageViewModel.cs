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

            foreach (var slot in slots)
            {
                if (answers.TryGetValue((slot.ItemId, slot.SlotKey), out var answer))
                    slot.LoadFrom(answer.ValueText, answer.ValueNum);

                slot.Changed += OnSlotChanged;
                _allSlots.Add(slot);

                if (slot.SlotKey == "value")
                    _valueSlots[item.Id] = slot;
            }

            var row = new ItemRowViewModel(item, slots, depth);

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

        void AddSection(FormSection section)
        {
            rowsOut.Add(new SectionRowViewModel(section));

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

        return rowsOut;
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
                return new SlotViewModel(itemId, slotKey, label, SlotKind.Choice,
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

        if (!_valueSlots.TryGetValue(itemId, out var slot))
            return false;

        string? current = slot.GetStored().Text;
        if (current == null)
            return false;

        return (values ?? string.Empty)
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Contains(current, StringComparer.OrdinalIgnoreCase);
    }

    // Itago o ipakita ang mga pangutana base sa show_if (pananglitan Two-wheeled / Three-wheeled).
    private void RefreshVisibility()
    {
        var sectionVisible = new Dictionary<string, bool>();
        foreach (var row in Rows.OfType<SectionRowViewModel>())
        {
            var s = row.Section;
            bool visible = Evaluate(s.ShowIfItemId, s.ShowIfValues)
                && (s.ParentId == null || sectionVisible.GetValueOrDefault(s.ParentId, true));
            sectionVisible[s.Id] = visible;
            row.IsVisible = visible;
        }

        var itemVisible = new Dictionary<string, bool>();
        foreach (var row in Rows.OfType<ItemRowViewModel>())
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
        if (!isTyping)
            UpdateScreen(slot.ItemId);

        _ = SaveSoonAsync();
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

        foreach (var row in Rows.OfType<ItemRowViewModel>())
            row.IsMissing = _highlightMissing && row.IsVisible && IsRowMissing(row, answers);
    }

    private static bool IsRowMissing(ItemRowViewModel row, IReadOnlyDictionary<(string, string), string> answers)
    {
        foreach (var slot in row.Slots)
        {
            bool counts = slot.SlotKey == "value"
                ? ProgressCalculator.ValueSlotCounts(row.Item)
                : slot.Kind == SlotKind.Choice;

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

        var target = Rows.OfType<ItemRowViewModel>().FirstOrDefault(r => r.IsMissing);
        if (target == null)
        {
            await Dialogs.ShowAsync("Nothing missing", "Every required item is filled in.");
            return;
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
