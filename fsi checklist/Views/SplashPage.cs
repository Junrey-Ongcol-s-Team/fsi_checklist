// =====================================================================
// SPLASH PAGE: ang BFP nga hulagway (Resources/Images/bfp_splash.png) human ma-tap ang app icon.
//   1. Ang Android/Windows mopakita una sa navy nga screen (MauiSplashScreen sa csproj, walay .NET logo).
//   2. Dayon kini nga page: ang BFP banner sulod sa 2 segundos.
//   3. Dayon ang AppShell (Home / Dashboard) sama sa una.
// Pinahigda (tablet): puno ang screen (AspectFill, maputol ra gamay ang kilid nga mga suga sa siyudad).
// Patindog (phone): tibuok hulagway makita (AspectFit) sa navy nga background.
// Usba ang SplashSeconds kung gusto og mas dugay o mas mubo.
// =====================================================================
namespace fsi_checklist.Views;

public sealed class SplashPage : ContentPage
{
    private const int SplashSeconds = 2;
    private readonly Image _image;
    private bool _started;

    public SplashPage()
    {
        NavigationPage.SetHasNavigationBar(this, false);
        Shell.SetNavBarIsVisible(this, false);

        // Navy nga background (gikan sa kolor sa hulagway): hayag sa ibabaw, ngitngit sa ubos
        Background = new LinearGradientBrush(
            new GradientStopCollection
            {
                new GradientStop(Color.FromArgb("#0B3A78"), 0f),
                new GradientStop(Color.FromArgb("#020A1E"), 1f),
            },
            new Point(0, 0), new Point(0, 1));

        _image = new Image
        {
            Source = "bfp_splash.png",
            Aspect = Aspect.AspectFit,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        };
        Content = new Grid { Children = { _image } };
        SizeChanged += (_, _) =>
            _image.Aspect = Width > 0 && Width / Math.Max(Height, 1) > 1.2 ? Aspect.AspectFill : Aspect.AspectFit;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_started)
            return;
        _started = true;

        await Task.Delay(TimeSpan.FromSeconds(SplashSeconds));
        if (Window != null)
            Window.Page = new AppShell();
    }
}
