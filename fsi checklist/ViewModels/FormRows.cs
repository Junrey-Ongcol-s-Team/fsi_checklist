// =====================================================================
// Ang mga piyesa sa form:
//   SlotViewModel        = usa ka tubag (text, numero, petsa, Yes/No...) = usa ka row sa inspection_answer
//   ChoiceOptionViewModel= usa ka chip (pananglitan 'Yes')
//   SectionRowViewModel  = ulohan sa seksyon
//   ItemRowViewModel     = usa ka pangutana (card) nga naay usa o daghang slot
// =====================================================================
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fsi_checklist.Data.Models;

namespace fsi_checklist.ViewModels;

public enum SlotKind { Text, LongText, Number, Date, Bool, Choice, Photo }

public sealed partial class ChoiceOptionViewModel : ObservableObject
{
    private readonly SlotViewModel _slot;

    public ChoiceOptionViewModel(SlotViewModel slot, string value, string label)
    {
        _slot = slot;
        Value = value;
        Label = label;
    }

    public string Value { get; }
    public string Label { get; }

    [ObservableProperty]
    private bool isSelected;

    [RelayCommand]
    private void Select() => _slot.Toggle(Value);
}

/// <summary>One answer slot of a form item: stored as one inspection_answer row (item + slot key).</summary>
public sealed partial class SlotViewModel : ObservableObject
{
    private bool _suppress;

    public SlotViewModel(
        string itemId,
        string slotKey,
        string? label,
        SlotKind kind,
        IEnumerable<(string Value, string Label)>? options = null,
        string? hint = null,
        bool isInteger = false)
    {
        ItemId = itemId;
        SlotKey = slotKey;
        Label = label;
        Kind = kind;
        Hint = hint;
        IsInteger = isInteger;
        Options = (options ?? Enumerable.Empty<(string, string)>())
            .Select(o => new ChoiceOptionViewModel(this, o.Value, o.Label))
            .ToList();
    }

    public string ItemId { get; }
    public string SlotKey { get; }
    public string? Label { get; }
    public bool HasLabel => !string.IsNullOrEmpty(Label);
    public SlotKind Kind { get; }
    public string? Hint { get; }
    public bool IsInteger { get; }
    public IReadOnlyList<ChoiceOptionViewModel> Options { get; }

    public event Action<SlotViewModel>? Changed;

    // Text, long text, number (as typed) and the selected choice value.
    [ObservableProperty]
    private string? text;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasNoDate))]
    private bool hasDate;

    [ObservableProperty]
    private DateTime dateValue = DateTime.Today;

    [ObservableProperty]
    private bool isChecked;

    public bool HasNoDate => !HasDate;

    partial void OnTextChanged(string? value)
    {
        if (Kind == SlotKind.Choice)
        {
            foreach (var option in Options)
                option.IsSelected = option.Value == value;
        }
        NotifyChanged();
    }

    partial void OnHasDateChanged(bool value) => NotifyChanged();
    partial void OnDateValueChanged(DateTime value) => NotifyChanged();
    partial void OnIsCheckedChanged(bool value) => NotifyChanged();

    [RelayCommand]
    private void SetToday() => Apply(() =>
    {
        DateValue = DateTime.Today;
        HasDate = true;
    });

    [RelayCommand]
    private void ClearDate() => Apply(() => HasDate = false);

    // Pag-tap sa napili na nga chip: i-clear ang tubag.
    /// <summary>Tapping the selected option again clears the answer.</summary>
    public void Toggle(string value) => Text = Text == value ? null : value;

    private void Apply(Action change)
    {
        _suppress = true;
        try { change(); }
        finally { _suppress = false; }
        NotifyChanged();
    }

    private void NotifyChanged()
    {
        if (!_suppress)
            Changed?.Invoke(this);
    }

    // Unsa ang i-save sa database para niini nga tubag (value_text / value_num). Kung parehas null = wala'y tubag.
    /// <summary>What goes into value_text / value_num for this slot (both null = no answer).</summary>
    public (string? Text, double? Num) GetStored()
    {
        switch (Kind)
        {
            case SlotKind.Number:
                if (double.TryParse(Text, NumberStyles.Float, CultureInfo.CurrentCulture, out double number)
                    && (!IsInteger || number == Math.Floor(number)))
                    return (null, number);
                return (null, null);

            case SlotKind.Date:
                return HasDate
                    ? (DateValue.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), null)
                    : (null, null);

            case SlotKind.Bool:
                // Only a ticked box is stored; value_text '1' is what show_if rules compare against.
                return IsChecked ? ("1", 1.0) : (null, null);

            case SlotKind.Photo:
                return (null, null);

            default:
                return string.IsNullOrWhiteSpace(Text) ? (null, null) : (Text.Trim(), null);
        }
    }

    // Ibutang balik sa screen ang tubag nga gikan sa database.
    public void LoadFrom(string? valueText, double? valueNum)
    {
        _suppress = true;
        try
        {
            switch (Kind)
            {
                case SlotKind.Number:
                    Text = valueNum?.ToString("G", CultureInfo.CurrentCulture);
                    break;

                case SlotKind.Date:
                    if (DateTime.TryParse(valueText, CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out var date))
                    {
                        DateValue = date;
                        HasDate = true;
                    }
                    break;

                case SlotKind.Bool:
                    IsChecked = valueNum == 1 || valueText == "1";
                    break;

                case SlotKind.Photo:
                    break;

                default:
                    Text = valueText;
                    break;
            }
        }
        finally
        {
            _suppress = false;
        }
    }
}

/// <summary>Usa ka litrato nga makita sa form (gamay nga hulagway).</summary>
public sealed class PhotoViewModel
{
    public PhotoViewModel(PhotoSlotViewModel slot, string id, string fullPath)
    {
        Slot = slot;
        Id = id;
        FullPath = fullPath;
    }

    public PhotoSlotViewModel Slot { get; }
    public string Id { get; }
    public string FullPath { get; }   // gamiton sa Image (Source)
}

/// <summary>
/// Usa ka lugar sa litrato sa pangutana. Pananglitan sa 064: "Front", "Rear", "Side".
/// Para sa Defects: usa ra ka lugar nga "Photos".
/// </summary>
public sealed class PhotoSlotViewModel
{
    public PhotoSlotViewModel(string itemId, string? tag, string label)
    {
        ItemId = itemId;
        Tag = tag;
        Label = label;
    }

    public string ItemId { get; }
    public string? Tag { get; }
    public string Label { get; }
    public System.Collections.ObjectModel.ObservableCollection<PhotoViewModel> Photos { get; } = new();
}

public abstract partial class FormRowViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isVisible = true;
}

public sealed class SectionRowViewModel : FormRowViewModel
{
    public SectionRowViewModel(FormSection section)
    {
        Section = section;
    }

    public FormSection Section { get; }
    public string Title => Section.Title;
}

public sealed partial class ItemRowViewModel : FormRowViewModel
{
    public ItemRowViewModel(FormItem item, IReadOnlyList<SlotViewModel> slots, int depth)
    {
        Item = item;
        Slots = slots;
        Margin = new Thickness(depth * 18, 0, 0, 0);
    }

    public FormItem Item { get; }
    public IReadOnlyList<SlotViewModel> Slots { get; }
    public Thickness Margin { get; }
    public string LabelText => Item.IsRequired ? $"{Item.Label} *" : Item.Label;
    public string? HelpText => Item.HelpText;
    public bool HasHelp => !string.IsNullOrWhiteSpace(Item.HelpText);

    // Mga lugar sa litrato (walay sulod kung dili kinahanglan og litrato ang pangutana).
    public List<PhotoSlotViewModel> PhotoSlots { get; } = new();
    public bool HasPhotoSlots => PhotoSlots.Count > 0;

    // Shown with a yellow frame after the user taps "Next missing".
    [ObservableProperty]
    private bool isMissing;
}
