// =====================================================================
// DASHBOARD: checkbox sa matag inspection card (makita ra kung "Select" mode).
// I-tick aron iapil sa Export.
//   <dash:SelectInspectionCheckBox IsChecked="{Binding IsSelected, Mode=TwoWay}" IsVisible="{Binding IsSelecting}" />
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.Dashboard;

public sealed class SelectInspectionCheckBox : CheckBox
{
    public SelectInspectionCheckBox()
    {
        Color = FieldParts.Primary;
        VerticalOptions = LayoutOptions.Center;
        Margin = new Thickness(0, 0, 6, 0);
    }
}
