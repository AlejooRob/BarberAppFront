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

        // Registrar el DelegatingHandler
        builder.Services.AddTransient<AuthHeaderHandler>();

        // Registrar las interfaces de Refit para tus APIs
        string baseUrl = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:8080" : "http://localhost:8080";

        builder.Services.AddRefitClient<IAuthService>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl + "/api/auth"));

        builder.Services.AddRefitClient<IUsuarioService>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl + "/api/usuarios"))
                .AddHttpMessageHandler<AuthHeaderHandler>();

        // Registrar tus ViewModels y Páginas
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<LoginPage>();

        builder.Services.AddSingleton<RegisterUserViewModel>();
        builder.Services.AddSingleton<RegisterUserPage>();

        // NUEVO: Registrar DashboardViewModel y DashboardPage
        builder.Services.AddSingleton<DashboardViewModel>();
        builder.Services.AddSingleton<DashboardPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}