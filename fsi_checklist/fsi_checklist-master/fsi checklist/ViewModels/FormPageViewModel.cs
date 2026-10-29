using CommunityToolkit.Mvvm.ComponentModel;
using fsi_checklist.Data.Models;
using fsi_checklist.Services;

namespace fsi_checklist.ViewModels;

[QueryProperty(nameof(InspectionId), "inspectionId")]
public sealed partial class FormPageViewModel : ObservableObject
{
    private readonly InspectionService _inspections;
    private readonly SemaphoreSlim _saveLock = new(1, 1);
    private readonly HashSet<SlotViewModel> _dirty = new();
    private readonly Dictionary<string, SlotViewModel> _valueSlots = new();
    private CancellationTokenSource? _debounce;

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

    partial void OnInspectionIdChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
            _ = LoadAsync();
    }

    // ----- loading -----------------------------------------------------------

    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var data = await _inspections.LoadFormAsync(InspectionId);
            if (data == null)
            {
                await Dialogs.ShowAsync("Not found", "This inspection no longer exists.");
                await Shell.Current.GoToAsync("..");
                return;
            }

            Title = data.Inspection.Template.DisplayName;
            Subtitle = BuildSubtitle(data.Inspection);
            Rows = BuildRows(data);
            RefreshVisibility();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await Dialogs.ShowAsync("Couldn't open inspection", ex.Message);
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

    private List<FormRowViewModel> BuildRows(FormData data)
    {
        _valueSlots.Clear();
        _dirty.Clear();

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

                if (slot.SlotKey == "value")
                    _valueSlots[item.Id] = slot;
            }

            rowsOut.Add(new ItemRowViewModel(item, slots, depth));

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
                    hint: $"Photos required: {slots}. Photo capture isn't available yet.");

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

    private void OnSlotChanged(SlotViewModel slot)
    {
        _dirty.Add(slot);
        SaveStatus = "Editing…";
        RefreshVisibility();
        _ = SaveSoonAsync();
    }

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

        await FlushAsync();
    }

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
                System.Diagnostics.Debug.WriteLine(ex);
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
}
