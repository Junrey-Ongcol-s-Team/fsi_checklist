using CommunityToolkit.Maui;
using UraniumUI;
using fsi_checklist.Data;
using fsi_checklist.Services;
using fsi_checklist.ViewModels;
using fsi_checklist.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace fsi_checklist
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseUraniumUI()
                .UseUraniumUIMaterial()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddDbContextFactory<AppDbContext>(
                options => options.UseSqlite(DatabaseService.ConnectionString));

            // services
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<InspectionService>();
            builder.Services.AddSingleton<AppSettingsService>();

            // pages & view models
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<InspectionPageViewModel>();
            builder.Services.AddTransient<InspectionPage>();
            builder.Services.AddTransient<NewInspectionViewModel>();
            builder.Services.AddTransient<NewInspectionPage>();
            builder.Services.AddTransient<FormPageViewModel>();
            builder.Services.AddTransient<FormPage>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<SettingsPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
