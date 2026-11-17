// =====================================================================
// FSIC RENEWAL (069): "N/A" BUTTON sa matag numero 1 - 13
// (Fire safety measures assessment of the building/structures/facility).
// I-tap kung wala ana ang establisyemento (pananglitan walay Fire Pump):
//   -> abo ang button, ug ang tanang pangutana sulod sa numero mawala (dili na tubagon)
//   -> sa print: "N/A" tapad sa "Compliant".
// I-tap pag-usab aron ibalik. Ang tubag = checkbox (BOOL) sa item nga "aNN.na" (seeds/069h.sql).
// =====================================================================
using fsi_checklist.Controls.Common;
using fsi_checklist.ViewModels;

namespace fsi_checklist.Controls.FsicRenewal;

public sealed class FsicRenewalNaButton : FormFieldBase
{
    private static readonly Color NaGray = Color.FromArgb("#616161");

    public override string FormName => "FSIC Renewal (069)";

    public FsicRenewalNaButton()
    {
        var button = FieldParts.Chip("N/A");
        button.HorizontalOptions = LayoutOptions.Start;
        FieldParts.Highlight(button, "IsChecked", NaGray);
        // I-tap: N/A <-> dili N/A
        button.Clicked += (_, _) =>
        {
            if (BindingContext is SlotViewModel slot)
                slot.IsChecked = !slot.IsChecked;
        };

        var hint = new Label
        {
            Text = "Tap N/A if the establishment doesn't have this. Its questions are skipped and the print shows N/A.",
            FontSize = 12,
            TextColor = Colors.Gray,
        };

        Finish(new VerticalStackLayout { Spacing = 4, Children = { button, hint } });
    }
}
