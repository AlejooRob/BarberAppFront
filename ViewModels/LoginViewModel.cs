using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BarberAppFront.Models;
using BarberAppFront.Services;
using BarberAppFront.Utils;
using BarberAppFront.Views; // Para navegar a otras páginas
using System; // Para Exception, IServiceProvider
using System.Threading.Tasks;
using Microsoft.Maui.Controls; // Para Application.Current.MainPage
using Refit; // Para ApiException

namespace BarberAppFront.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly IServiceProvider _serviceProvider; // Inyectamos IServiceProvider

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage;

        [ObservableProperty]
        private bool hasError;

        // Modificamos el constructor: ya no inyectamos RegisterUserPage directamente
        public LoginViewModel(IAuthService authService, IServiceProvider serviceProvider)
        {
            _authService = authService;
            _serviceProvider = serviceProvider;
        }

        [RelayCommand]
        private async Task Login()
        {
            IsBusy = true;
            HasError = false;
            ErrorMessage = string.Empty;

            try
            {
                var request = new LoginRequest { Username = Username, Password = Password };
                var response = await _authService.Login(request);

                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    await SecureStorageHelper.SaveTokenAsync(response.Content.Token);

                    //string tokenGuardado = await SecureStorageHelper.GetTokenAsync();
                    //System.Diagnostics.Debug.WriteLine($"[DEBUG] Token guardado y recuperado: {tokenGuardado}");

                    //Guardar rol
                    Preferences.Set("userRole", response.Content.Role);
                    Preferences.Set("userId", response.Content.Id.ToString());

                    // Navegar a la página principal del dashboard
                    Application.Current.MainPage = new NavigationPage(_serviceProvider.GetService<DashboardPage>());
                  
                }
                else
                {
                    ErrorMessage = "Credenciales inválidas o error al iniciar sesión.";
                    if (response.Error?.Content != null)
                    {
                        try
                        {
                            MessageResponse errorParsed = System.Text.Json.JsonSerializer.Deserialize<MessageResponse>(response.Error.Content);
                            ErrorMessage = errorParsed?.Message ?? ErrorMessage;
                        }
                        catch { /* Ignorar error de parsing si el contenido no es un MessageResponse */ }
                    }
                    HasError = true;
                }
            }
            catch (ApiException ex)
            {
                ErrorMessage = $"Error de API: {ex.StatusCode}. {ex.Content}";
                HasError = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ocurrió un error inesperado: {ex.Message}";
                HasError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Comando para navegar a la pantalla de registro (Admin)
        [RelayCommand]
        private async Task GoToRegisterUser()
        {
            if (Application.Current.MainPage is NavigationPage navigationPage)
            {
                // Resolve RegisterUserPage usando el ServiceProvider
                var registerUserPage = _serviceProvider.GetService<RegisterUserPage>();
                if (registerUserPage != null)
                {
                    await navigationPage.PushAsync(registerUserPage);
                }
                else
                {
                    // Manejar el caso si la página no se pudo resolver (no está registrada)
                    await Application.Current.MainPage.DisplayAlert("Error", "La página de registro no está disponible.", "OK");
                }
            }
        }
    }
}