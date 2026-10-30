// =====================================================================
// NAV: usa ka lugar para sa tanang pag-navigate (pagbalhin sa page). SAYON I-DEBUG:
//   - matag pag-navigate gisulat sa app.log ("NAV -> ...")
//   - kung mapakyas, isulat ang error ug ipakita sa user (dili mo-freeze nga hilom)
// =====================================================================
using fsi_checklist.Views;

namespace fsi_checklist.Services;

public static class Nav
{
    // BALIK SA DASHBOARD gikan bisan asa.
    // Sa Windows, ang "../.." ug "//MainPage/DashboardPage" usahay mo-stuck nga hilom.
    // Busa: pangitaon ang Dashboard sa stack, tangtangon ang mga page sa ibabaw niini, dayon mobalik (PopAsync).
    public static async Task BackToDashboardAsync()
    {
        var nav = Shell.Current.Navigation;
        var pages = nav.NavigationStack.ToList();
        int dashboard = pages.FindLastIndex(p => p is DashboardPage);

        AppLog.Info($"NAV -> back to Dashboard (stack: {string.Join(" > ", pages.Select(p => p?.GetType().Name ?? "root"))})");

        try
        {
            if (dashboard < 0)
            {
                // Wala ang Dashboard sa stack: ablihan og bag-o.
                await Shell.Current.GoToAsync(nameof(DashboardPage));
                return;
            }

            // Tangtangon ang mga page tali sa Dashboard ug sa kasamtangan nga page...
            for (int i = pages.Count - 2; i > dashboard; i--)
                nav.RemovePage(pages[i]);

            // ...dayon isira ang kasamtangan nga page.
            if (pages.Count - 1 > dashboard)
                await nav.PopAsync();
        }
        catch (Exception ex)
        {
            AppLog.Error(ex, "Back to Dashboard failed");
            await Dialogs.ShowAsync("Couldn't go back", AppLog.Describe(ex));
        }
    }

    public static async Task GoAsync(string route)
    {
        AppLog.Info($"NAV -> {route}");
        try
        {
            await Shell.Current.GoToAsync(route);
        }
        catch (Exception ex)
        {
            AppLog.Error(ex, $"Navigation failed: {route}");
            await Dialogs.ShowAsync("Couldn't open the page", AppLog.Describe(ex));
        }
    }
}
