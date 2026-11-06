// =====================================================================
// COMMON: ang PUNDASYON (base) sa tanang kaugalingong controls sa matag form.
//
// Ang matag form naay kaugalingong folder sa Controls (pananglitan Controls/FsicRenewal).
// Ang mga control didto mo-extend (mo-sunod) sa usa niini nga base:
//   TextFieldBase      = kahon nga sulatan (naay N/A button kung gitugotan)
//   LongTextFieldBase  = taas nga sulatanan (Remarks, Defects)
//   NumberFieldBase    = numero ra + unit sa tuo (m, persons ...)
//   DateFieldBase      = petsa (Set date / Clear)
//   CheckFieldBase     = checkbox
//   ChoiceFieldBase    = mga chip, usa ra ang mapili
//   YesNoFieldBase     = Yes (berde) / No (pula) / N/A (abo)
//   ChipsFieldBase     = daghan nga chip + search (Fire Safety Clearances)
//   PickerFieldBase    = dropdown (Type of occupancy)
//   PhotoNoteBase      = pahibalo kung unsang litrato ang kinahanglan
//
// Ang BASE ang naghimo sa layout ug binding sa SlotViewModel.
// Ang CONTROL SA FORM ang mo-ingon sa iyang ngalan, title, placeholder ug kolor.
// =====================================================================
using fsi_checklist.ViewModels;

namespace fsi_checklist.Controls.Common;

/// <summary>Gagmay nga piyesa nga gamiton sa tanang base (label, chip, kolor).</summary>
internal static class FieldParts
{
    // Ang "Primary" nga kolor sa app (Resources/Styles/Colors.xaml)
    public static Color Primary =>
        Application.Current?.Resources.TryGetValue("Primary", out var value) == true && value is Color color
            ? color
            : Color.FromArgb("#512BD4");

    // Gamay nga label sa ibabaw sa kahon (pananglitan "Date issued")
    public static Label Caption()
    {
        var label = new Label { FontSize = 12 };
        label.SetBinding(Label.TextProperty, "Label");
        label.SetBinding(VisualElement.IsVisibleProperty, "HasLabel");
        return label;
    }

    // Lingin nga button (chip). filled = puno na og kolor (pananglitan ang custom clearance)
    public static Button Chip(string? text = null, bool filled = false)
    {
        var button = new Button
        {
            HeightRequest = 48,
            CornerRadius = 24,
            Padding = new Thickness(26, 0),
            FontSize = 15,
            BorderWidth = 1,
        };
        if (text != null)
            button.Text = text;

        if (filled)
        {
            button.BackgroundColor = Primary;
            button.BorderColor = Primary;
            button.TextColor = Colors.White;
        }
        else
        {
            button.BackgroundColor = Colors.Transparent;
            button.SetAppThemeColor(Button.BorderColorProperty, Color.FromArgb("#BDBDBD"), Color.FromArgb("#5A5A5A"));
            button.SetAppThemeColor(Button.TextColorProperty, Color.FromArgb("#424242"), Color.FromArgb("#E0E0E0"));
        }
        return button;
    }

    // Kung TRUE ang 'path' (pananglitan IsSelected), puno og kolor ang chip ug puti ang letra
    public static void Highlight(Button button, string path, Color color)
    {
        var trigger = new DataTrigger(typeof(Button)) { Binding = new Binding(path), Value = true };
        trigger.Setters.Add(new Setter { Property = VisualElement.BackgroundColorProperty, Value = color });
        trigger.Setters.Add(new Setter { Property = Button.BorderColorProperty, Value = color });
        trigger.Setters.Add(new Setter { Property = Button.TextColorProperty, Value = Colors.White });
        button.Triggers.Add(trigger);
    }

    // Duha ka column: wala = mopuno, tuo = sakto ra (pananglitan Entry + N/A)
    public static Grid TwoColumns(View left, View right)
    {
        var grid = new Grid { ColumnSpacing = 8 };
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        grid.Add(left, 0, 0);
        grid.Add(right, 1, 0);
        return grid;
    }
}

/// <summary>Ang pinaka-ubos nga base: matag control kabalo kung asang form siya.</summary>
public abstract class FormFieldBase : ContentView
{
    // Ngalan sa form (pananglitan "FSIC Renewal (069)")
    public abstract string FormName { get; }

    // Ibutang ang layout. AutomationId = "FSIC Renewal (069)/FsicRenewalDateField" (para sa testing)
    protected void Finish(View body)
    {
        Content = body;
        AutomationId = $"{FormName}/{GetType().Name}";
    }
}

/// <summary>Kahon nga sulatan. Naay N/A button kung CanNa (Fire Drill, Fire Insurance).</summary>
public abstract class TextFieldBase : FormFieldBase
{
    protected virtual string NaText => "N/A";

    protected TextFieldBase()
    {
        var entry = new Entry();
        entry.SetBinding(Entry.PlaceholderProperty, "Hint");
        entry.SetBinding(Entry.TextProperty, "Text");

        var na = FieldParts.Chip(NaText);
        na.SetBinding(Button.CommandProperty, "ToggleNaCommand");
        na.SetBinding(VisualElement.IsVisibleProperty, "CanNa");
        FieldParts.Highlight(na, "IsNa", FieldParts.Primary);

        Finish(new VerticalStackLayout
        {
            Spacing = 2,
            Children = { FieldParts.Caption(), FieldParts.TwoColumns(entry, na) },
        });
    }
}

/// <summary>Taas nga sulatanan (Remarks, Defects, Recommendations).</summary>
public abstract class LongTextFieldBase : FormFieldBase
{
    protected LongTextFieldBase()
    {
        var editor = new Editor { AutoSize = EditorAutoSizeOption.TextChanges, MinimumHeightRequest = 80 };
        editor.SetBinding(Editor.TextProperty, "Text");

        Finish(new VerticalStackLayout
        {
            Spacing = 2,
            Children = { FieldParts.Caption(), editor },
        });
    }
}

/// <summary>Numero ra + UNIT sa tuo (pananglitan "___ m", "___ persons").</summary>
public abstract class NumberFieldBase : FormFieldBase
{
    protected NumberFieldBase()
    {
        var entry = new Entry { Keyboard = Keyboard.Numeric, Placeholder = "0" };
        entry.SetBinding(Entry.TextProperty, "Text");

        var unit = new Label { FontSize = 14, VerticalOptions = LayoutOptions.Center };
        unit.SetBinding(Label.TextProperty, "Hint");
        unit.SetBinding(VisualElement.IsVisibleProperty, "HasHint");

        var layout = new VerticalStackLayout
        {
            Spacing = 2,
            Children = { FieldParts.Caption(), FieldParts.TwoColumns(entry, unit) },
        };
        // Matago kung N/A ang parent (pananglitan Capacity)
        layout.SetBinding(VisualElement.IsVisibleProperty, "IsShown");
        Finish(layout);
    }
}

/// <summary>Petsa: "Set date" kung wala pa, DatePicker + "Clear" kung naa na.</summary>
public abstract class DateFieldBase : FormFieldBase
{
    protected virtual string SetDateText => "Set date";

    protected DateFieldBase()
    {
        var picker = new DatePicker();
        picker.SetBinding(DatePicker.DateProperty, "DateValue");
        picker.SetBinding(VisualElement.IsVisibleProperty, "HasDate");

        var setDate = new Button { Text = SetDateText };
        setDate.SetBinding(Button.CommandProperty, "SetTodayCommand");
        setDate.SetBinding(VisualElement.IsVisibleProperty, "HasNoDate");

        var clear = new Button { Text = "Clear" };
        clear.SetBinding(Button.CommandProperty, "ClearDateCommand");
        clear.SetBinding(VisualElement.IsVisibleProperty, "HasDate");

        Finish(new VerticalStackLayout
        {
            Spacing = 2,
            Children =
            {
                FieldParts.Caption(),
                new HorizontalStackLayout { Spacing = 8, Children = { picker, setDate, clear } },
            },
        });
    }
}

/// <summary>Checkbox (pananglitan "For Issuance of", mga kahon nga i-check).</summary>
public abstract class CheckFieldBase : FormFieldBase
{
    protected CheckFieldBase()
    {
        var box = new CheckBox { HorizontalOptions = LayoutOptions.Start };
        box.SetBinding(CheckBox.IsCheckedProperty, "IsChecked");

        Finish(new VerticalStackLayout
        {
            Spacing = 2,
            Children = { FieldParts.Caption(), box },
        });
    }
}

/// <summary>Mga chip, usa ra ang mapili (pananglitan Nature of inspection).</summary>
public abstract class ChoiceFieldBase : FormFieldBase
{
    // Kolor sa chip kung napili. Ang YesNo mo-usab niini (Yes = berde, No = pula).
    protected virtual Color ColorFor(string value) => FieldParts.Primary;

    protected ChoiceFieldBase()
    {
        var chips = new FlexLayout { Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap };
        chips.SetBinding(BindableLayout.ItemsSourceProperty, "Options");
        BindableLayout.SetItemTemplate(chips, new DataTemplate(() => MakeChip()));

        Finish(new VerticalStackLayout
        {
            Spacing = 4,
            Children = { FieldParts.Caption(), chips },
        });
    }

    private Button MakeChip()
    {
        var chip = FieldParts.Chip();
        chip.Margin = new Thickness(0, 0, 8, 8);
        chip.SetBinding(Button.CommandProperty, "SelectCommand");
        chip.SetBinding(Button.TextProperty, "Label");

        // Ang kolor depende sa pilian (Yes/No/N/A), busa ibutang human mahibal-an ang pilian
        chip.BindingContextChanged += (_, _) =>
        {
            chip.Triggers.Clear();
            if (chip.BindingContext is ChoiceOptionViewModel option)
                FieldParts.Highlight(chip, "IsSelected", ColorFor(option.Value));
        };
        return chip;
    }
}

/// <summary>Yes / No / N/A (ug Passed / Failed): berde, pula, abo.</summary>
public abstract class YesNoFieldBase : ChoiceFieldBase
{
    protected virtual Color YesColor => Color.FromArgb("#2E7D32");   // berde
    protected virtual Color NoColor => Color.FromArgb("#C62828");    // pula
    protected virtual Color NaColor => Color.FromArgb("#616161");    // abo

    protected override Color ColorFor(string value) => value switch
    {
        "YES" or "PASS" => YesColor,
        "NO" or "FAIL" => NoColor,
        "NA" => NaColor,
        _ => FieldParts.Primary,
    };
}

/// <summary>Daghan nga chip + search + i-type ang lain (Fire Safety Clearances).</summary>
public abstract class ChipsFieldBase : FormFieldBase
{
    protected virtual string SearchPlaceholder => "Search or type";

    protected ChipsFieldBase()
    {
        var search = new Entry { Placeholder = SearchPlaceholder };
        search.SetBinding(Entry.ReturnCommandProperty, "AddChipCommand");
        search.SetBinding(Entry.TextProperty, "NewChip");

        var add = new Button { Text = "Add" };
        add.SetBinding(Button.CommandProperty, "AddChipCommand");

        // Ang andam nga mga chip (mo-filter samtang nag-type)
        var options = new FlexLayout { Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap };
        options.SetBinding(BindableLayout.ItemsSourceProperty, "FilteredOptions");
        BindableLayout.SetItemTemplate(options, new DataTemplate(() =>
        {
            var chip = FieldParts.Chip();
            chip.Margin = new Thickness(0, 0, 8, 8);
            chip.SetBinding(Button.CommandProperty, "SelectCommand");
            chip.SetBinding(Button.TextProperty, "Label");
            FieldParts.Highlight(chip, "IsSelected", FieldParts.Primary);
            return chip;
        }));

        // Ang gi-type sa user (wala sa listahan), i-tap aron tangtangon
        var custom = new FlexLayout { Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap };
        custom.SetBinding(BindableLayout.ItemsSourceProperty, "CustomChips");
        BindableLayout.SetItemTemplate(custom, new DataTemplate(() =>
        {
            var chip = FieldParts.Chip(filled: true);
            chip.Margin = new Thickness(0, 0, 8, 8);
            chip.SetBinding(Button.CommandProperty, "SelectCommand");
            chip.SetBinding(Button.TextProperty, "Label");
            return chip;
        }));

        Finish(new VerticalStackLayout
        {
            Spacing = 4,
            Children = { FieldParts.Caption(), FieldParts.TwoColumns(search, add), options, custom },
        });
    }
}

/// <summary>Dropdown para sa taas nga listahan (pananglitan Type of occupancy).</summary>
public abstract class PickerFieldBase : FormFieldBase
{
    protected virtual string PickerTitle => "Select";

    protected PickerFieldBase()
    {
        var picker = new Picker { Title = PickerTitle };
        picker.SetBinding(Picker.ItemsSourceProperty, "OptionLabels");
        picker.SetBinding(Picker.SelectedIndexProperty, new Binding("PickerIndex", BindingMode.TwoWay));

        Finish(new VerticalStackLayout
        {
            Spacing = 2,
            Children = { FieldParts.Caption(), picker },
        });
    }
}

/// <summary>Pahibalo kung unsang litrato ang kinahanglan (ang buttons naa sa item card).</summary>
public abstract class PhotoNoteBase : FormFieldBase
{
    protected PhotoNoteBase()
    {
        var note = new Label { FontAttributes = FontAttributes.Italic, FontSize = 13 };
        note.SetBinding(Label.TextProperty, "Hint");
        Finish(note);
    }
}
