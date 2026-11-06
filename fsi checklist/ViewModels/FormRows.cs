// =====================================================================
// Ang mga piyesa sa form:
//   SlotViewModel        = usa ka tubag (text, numero, petsa, Yes/No...) = usa ka row sa inspection_answer
//   ChoiceOptionViewModel= usa ka chip (pananglitan 'Yes')
//   SectionRowViewModel  = ulohan sa seksyon
//   ItemRowViewModel     = usa ka pangutana (card) nga naay usa o daghang slot
// =====================================================================
using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fsi_checklist.Data.Models;

namespace fsi_checklist.ViewModels;

public enum SlotKind { Text, LongText, Number, Date, Bool, Choice, Photo, Chips, Picker }

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
        if (kind == SlotKind.Chips)
            FilterChips();
    }

    public string ItemId { get; }
    public string SlotKey { get; }
    public string? Label { get; }
    public bool HasLabel => !string.IsNullOrEmpty(Label);
    public SlotKind Kind { get; }
    public string? Hint { get; }

    // Nakatago ang slot (pananglitan ang numero kung N/A ang gipili).
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsShown))]
    private bool isHidden;

    public bool IsShown => !IsHidden;

    // UNIT sa numero (pananglitan "m", "persons"): makita kanunay sa tuo sa numero.
    public bool HasHint => !string.IsNullOrWhiteSpace(Hint);
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
        // NUMERO RA: tangtangon dayon ang letra ug ubang simbolo (pananglitan "12a" -> "12").
        // Integer = digits ra; Number = digits ug usa ka decimal point (pananglitan 2.5).
        if (Kind == SlotKind.Number && value != null)
        {
            string clean = OnlyNumber(value, IsInteger);
            if (clean != value)
            {
                Text = clean;   // modagan pag-usab ang OnTextChanged uban ang limpyo nga numero
                return;
            }
        }

        if (Kind == SlotKind.Choice || Kind == SlotKind.Picker)
        {
            foreach (var option in Options)
                option.IsSelected = option.Value == value;
        }
        if (Kind == SlotKind.Picker)
            OnPropertyChanged(nameof(PickerIndex));
        if (Kind == SlotKind.Chips)
            SyncChips();
        OnPropertyChanged(nameof(IsNa));
        NotifyChanged();
    }

    // ----- PICKER (dropdown): pananglitan 069 "Type of occupancy" -----
    public List<string> OptionLabels => Options.Select(o => o.Label).ToList();

    public int PickerIndex
    {
        get
        {
            for (int i = 0; i < Options.Count; i++)
                if (Options[i].Value == Text)
                    return i;
            return -1;
        }
        set
        {
            string? v = value >= 0 && value < Options.Count ? Options[value].Value : null;
            if (v != Text)
                Text = v;
        }
    }

    // ----- N/A button (pananglitan Certificate of Fire Drill, Fire Insurance Policy No.) -----
    // CanNa = naay "N/A" button tapad sa sulatanan. I-tap: "N/A" ang tubag; i-tap pag-usab: mawala.
    public bool CanNa { get; set; }
    public bool IsNa => string.Equals(Text?.Trim(), "N/A", StringComparison.OrdinalIgnoreCase);

    [RelayCommand]
    private void ToggleNa() => Text = IsNa ? null : "N/A";

    // ----- CHIPS (pananglitan "Secure the following Fire Safety Clearance/s") -----
    // Ang tubag = mga chip nga gibulag og bag-ong linya (matag chip = usa ka linya sa print).
    // Options = andam nga mga chip; CustomChips = ang gi-type sa user (wala sa listahan).
    public ObservableCollection<ChoiceOptionViewModel> CustomChips { get; } = new();

    // SEARCH: ang mga chip nga makita (mo-filter samtang nag-type sa box).
    public ObservableCollection<ChoiceOptionViewModel> FilteredOptions { get; } = new();

    [ObservableProperty]
    private string? newChip;

    partial void OnNewChipChanged(string? value) => FilterChips();

    private void FilterChips()
    {
        string q = (NewChip ?? "").Trim();
        FilteredOptions.Clear();
        foreach (var o in Options.Where(o => q.Length == 0
                     || o.Label.Contains(q, StringComparison.OrdinalIgnoreCase)
                     || o.Value.Contains(q, StringComparison.OrdinalIgnoreCase)))
            FilteredOptions.Add(o);
    }

    private List<string> ChipValues() =>
        (Text ?? "").Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();

    private void SyncChips()
    {
        var values = ChipValues();
        foreach (var option in Options)
            option.IsSelected = values.Contains(option.Value, StringComparer.OrdinalIgnoreCase);
        CustomChips.Clear();
        foreach (var v in values.Where(v => !Options.Any(o => string.Equals(o.Value, v, StringComparison.OrdinalIgnoreCase))))
            CustomChips.Add(new ChoiceOptionViewModel(this, v, v + "  \u2715") { IsSelected = true });
    }

    // I-add ang gi-type nga chip (pananglitan "Bonfire").
    [RelayCommand]
    private void AddChip()
    {
        string v = (NewChip ?? "").Trim();
        NewChip = "";
        if (v.Length == 0)
            return;
        // Kung pareho sa usa sa listahan (o usa ra ang nakita sa search): kana ang pilion
        var match = Options.FirstOrDefault(o => string.Equals(o.Label, v, StringComparison.OrdinalIgnoreCase)
                                             || string.Equals(o.Value, v, StringComparison.OrdinalIgnoreCase));
        var found = Options.Where(o => o.Label.Contains(v, StringComparison.OrdinalIgnoreCase)).ToList();
        if (match == null && found.Count == 1)
            match = found[0];
        string value = match?.Value ?? v;
        if (!ChipValues().Contains(value, StringComparer.OrdinalIgnoreCase))
            Toggle(value);
    }

    // Digits ra (ug usa ka decimal point kung dili integer). Ang "," o "." pareho nga decimal point.
    private static string OnlyNumber(string value, bool integer)
    {
        string dec = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        var sb = new System.Text.StringBuilder();
        bool hasDot = false;
        foreach (char ch in value)
        {
            if (char.IsAsciiDigit(ch))
                sb.Append(ch);
            else if (!integer && !hasDot && (ch == '.' || ch == ','))
            {
                sb.Append(dec);
                hasDot = true;
            }
        }
        return sb.ToString();
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
    public void Toggle(string value)
    {
        if (Kind == SlotKind.Chips)
        {
            // Chips: idugang kung wala pa, tangtangon kung naa na
            var values = ChipValues();
            int at = values.FindIndex(v => string.Equals(v, value, StringComparison.OrdinalIgnoreCase));
            if (at >= 0)
                values.RemoveAt(at);
            else
                values.Add(value);
            Text = values.Count == 0 ? null : string.Join("\n", values);
            return;
        }
        Text = Text == value ? null : value;
    }

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
    public SectionRowViewModel(FormSection section, string? title = null)
    {
        Section = section;
        _title = title;
    }

    private readonly string? _title;
    public FormSection Section { get; }
    // Ang ulohan nga makita (pananglitan "Fire protection" ra imbes "Fire protection: A. ...").
    public string Title => _title ?? Section.Title;
}

public sealed partial class ItemRowViewModel : FormRowViewModel
{
    public ItemRowViewModel(FormItem item, IReadOnlyList<SlotViewModel> slots, int depth, bool isExpandable = false)
    {
        Item = item;
        Slots = slots;
        Margin = new Thickness(depth * 18, 0, 0, 0);
        IsExpandable = isExpandable;
    }

    // EXPANDER (UraniumUI ExpanderView): ang numero (pananglitan "1. Building Construction") = Header,
    // ug ang mga pangutana sulod niini (Children) = body. Sirado sa sugod; i-tap ang header aron moabli.
    public bool IsExpandable { get; }

    // Mga pangutana sulod sa expander (apil ang apo, sama sa Renovation ubos sa With changes).
    public List<ItemRowViewModel> Children { get; } = new();

    // Naka-bind (TwoWay) sa ExpanderView.IsExpanded, aron ang 'Next missing' makaabli niini.
    [ObservableProperty]
    private bool isExpanded;

    // DONE nga button sa ubos sa expander: sirad-an ug mobalik ang screen sa numero (dili na mo-scroll pataas).
    public event Action<ItemRowViewModel>? Collapsed;

    [RelayCommand]
    private void Collapse()
    {
        IsExpanded = false;
        Collapsed?.Invoke(this);
    }

    // CIRCLE PROGRESS sa header: pila na ang natubag sulod niini nga numero.
    [ObservableProperty]
    private double progress;          // 0.0 hangtod 1.0

    [ObservableProperty]
    private string progressText = string.Empty;   // "2/3", o "✓" kung kompleto

    [ObservableProperty]
    private Color ringColor = Color.FromArgb("#F9A825");

    [ObservableProperty]
    private bool hasProgress;         // false kung walay kinahanglan tubagon sulod niini

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
