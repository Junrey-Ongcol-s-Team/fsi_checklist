// =====================================================================
// Ang pagsugod sa app. Dinhi gi-register ang database, mga service, ug tanang page + ViewModel (dependency injection).
// Kung mag-add ka og bag-ong page: AddTransient diri, ug RegisterRoute sa AppShell.xaml.cs.
// =====================================================================
using CommunityToolkit.Maui;
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
            // CRASH LOGGER: kung naay error nga walay nakasalo (pananglitan NullReferenceException),
            // isulat kini sa app.log uban ang tibuok stack trace (asa nga file ug linya).
            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                    fsi_checklist.Services.AppLog.Error(ex, "UNHANDLED");
            };
            TaskScheduler.UnobservedTaskException += (_, e) =>
            {
                fsi_checklist.Services.AppLog.Error(e.Exception, "UNOBSERVED TASK");
                e.SetObserved();
            };

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddDbContextFactory<AppDbContext>(
                options => options.UseSqlite(DatabaseService.ConnectionString));

            // services
            // Singleton = usa ra ka kopya sa tibuok app (database ug mga service).
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<InspectionService>();
            builder.Services.AddSingleton<AppSettingsService>();

            // pages & view models
            // Transient = bag-ong kopya matag abli sa page (mga page ug ViewModel).
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<InspectionPageViewModel>();
            builder.Services.AddTransient<InspectionPage>();
            builder.Services.AddTransient<NewInspectionViewModel>();
            builder.Services.AddTransient<NewInspectionPage>();
            builder.Services.AddTransient<DashboardPageViewModel>();
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<ChecklistTypesViewModel>();
            builder.Services.AddTransient<ChecklistTypesPage>();
            builder.Services.AddTransient<SignoffPageViewModel>();
            builder.Services.AddTransient<SignoffPage>();
            builder.Services.AddTransient<SignaturePageViewModel>();
            builder.Services.AddTransient<SignaturePage>();
            builder.Services.AddTransient<ResultsPageViewModel>();
            builder.Services.AddTransient<ResultsPage>();
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
