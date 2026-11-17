// =====================================================================
// DASHBOARD: "Edit" button (outline). STEP 1: detalye (ngalan, address, contact ...), STEP 2: ang form (entries).
// Pwede KANUNAY (bisan na-Complete ug naka-pirma na ang Chief FSES ug Fire Marshal),
// gawas kung LOCKED na ang inspection (tago kini nga button kung locked).
// =====================================================================
namespace fsi_checklist.Controls.Dashboard;

public sealed class EditEntriesButton : InspectionActionButton
{
    public EditEntriesButton() : base("Edit")
    {
        BackgroundColor = Colors.Transparent;
        BorderColor = Primary;
        BorderWidth = 1;
        TextColor = Primary;
    }
}
