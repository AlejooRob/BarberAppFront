using BarberAppFront;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Refit;
using BarberAppFront.Services;
using BarberAppFront.Models;
using BarberAppFront.ViewModels;
using BarberAppFront.Views;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddTransient<AuthHeaderHandler>();

        string baseUrl = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:8080" : "http://localhost:8080";

        builder.Services.AddRefitClient<IAuthService>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl + "/api/auth"));

        builder.Services.AddRefitClient<IUsuarioService>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl + "/api/usuarios"))
                .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IPrestacionServicioService>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl + "/api/prestaciones"))
                .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IServicioService>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl + "/api/servicios"))
                .AddHttpMessageHandler<AuthHeaderHandler>();

        // Registrar ViewModels y Páginas
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<LoginPage>();

        builder.Services.AddSingleton<RegisterUserViewModel>();
        builder.Services.AddSingleton<RegisterUserPage>();

        builder.Services.AddSingleton<DashboardViewModel>();
        builder.Services.AddSingleton<DashboardPage>();

        builder.Services.AddSingleton<PrestacionesViewModel>();
        builder.Services.AddSingleton<PrestacionesPage>();

        // NUEVO: Registrar AddEditPrestacionViewModel y AddEditPrestacionPage
        builder.Services.AddSingleton<AddEditPrestacionViewModel>();
        builder.Services.AddSingleton<AddEditPrestacionPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}