// =====================================================================
// DASHBOARD: "Lock this inspection" button (pula nga outline).
// I-tap: pangutan-on una, dayon i-lock. Human niini DILI NA MA-EDIT (tubag, litrato, pirma).
// Kung locked na: "Locked" (abo) ug dili na ma-tap.
//   <dash:LockInspectionButton IsLocked="{Binding IsLocked}" Clicked="OnLockClicked" />
// =====================================================================
namespace fsi_checklist.Controls.Dashboard;

public sealed class LockInspectionButton : InspectionActionButton
{
    private static readonly Color LockRed = Color.FromArgb("#C62828");
    private static readonly Color LockedGray = Color.FromArgb("#757575");

    // IsLocked = true kung gi-lock na ang inspection
    public static readonly BindableProperty IsLockedProperty = BindableProperty.Create(
        nameof(IsLocked), typeof(bool), typeof(LockInspectionButton), false,
        propertyChanged: (bindable, _, _) => ((LockInspectionButton)bindable).Paint());

    public bool IsLocked
    {
        get => (bool)GetValue(IsLockedProperty);
        set => SetValue(IsLockedProperty, value);
    }

    public LockInspectionButton() : base("\U0001F512 Lock this inspection")
    {
        BorderWidth = 1;
        Paint();
    }

    // Usbon ang text ug kolor depende kung locked na ba
    private void Paint()
    {
        Text = IsLocked ? "\U0001F512 Locked" : "\U0001F512 Lock this inspection";
        IsEnabled = !IsLocked;
        BackgroundColor = Colors.Transparent;
        BorderColor = IsLocked ? LockedGray : LockRed;
        TextColor = IsLocked ? LockedGray : LockRed;
    }
}
