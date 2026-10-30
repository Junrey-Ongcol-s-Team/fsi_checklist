// =====================================================================
// Gamay nga helper para sa mga popup: ShowAsync = OK ra, ConfirmAsync = Oo/Dili.
// =====================================================================
namespace fsi_checklist.Services;

/// <summary>Thin wrapper so alert calls live in one place.</summary>
public static class Dialogs
{
    public static Task ShowAsync(string title, string message) =>
        Shell.Current.DisplayAlertAsync(title, message, "OK");

    public static Task<bool> ConfirmAsync(
        string title, string message, string accept = "Delete", string cancel = "Cancel") =>
        Shell.Current.DisplayAlertAsync(title, message, accept, cancel);
}
