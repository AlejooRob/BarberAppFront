using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BarberAppFront.Views;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui; // Para Preferences

namespace BarberAppFront.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private string welcomeMessage;

        [ObservableProperty]
        private bool isAdmin; // Propiedad para controlar la visibilidad del botón de Admin

        public DashboardViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            LoadUserRole(); // Cargar el rol del usuario al inicializar el ViewModel
        }

        // Método para cargar el rol y decidir la visibilidad del botón
        private void LoadUserRole()
        {
            string userRole = Preferences.Get("userRole", string.Empty);
            if (userRole.Equals("ADMIN", StringComparison.OrdinalIgnoreCase))
            {
                IsAdmin = true;
                WelcomeMessage = "¡Bienvenido, Administrador!";
            }
            else if (userRole.Equals("BARBERO", StringComparison.OrdinalIgnoreCase))
            {
                IsAdmin = false; // O si el barbero necesita ver algo específico de su rol
                WelcomeMessage = "¡Bienvenido, Barbero!";
            }
            else
            {
                WelcomeMessage = "Bienvenido."; // Para otros roles o si no se encontró el rol
            }
        }

        // Comando para navegar a la pantalla de registro de usuario (solo ADMIN)
        [RelayCommand]
        private async Task GoToRegisterUser()
        {
            // Solo permitir la navegación si el usuario es Admin (doble chequeo)
            if (IsAdmin && Application.Current.MainPage is NavigationPage navigationPage)
            {
                var registerUserPage = _serviceProvider.GetService<RegisterUserPage>();
                if (registerUserPage != null)
                {
                    await navigationPage.PushAsync(registerUserPage);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "La página de registro no está disponible.", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Acceso Denegado", "Solo los administradores pueden registrar nuevos usuarios.", "OK");
            }
        }

        // Comandos de navegación para otras secciones (implementar más adelante)
        [RelayCommand]
        private async Task GoToPrestaciones()
        {
            await Application.Current.MainPage.DisplayAlert("Funcionalidad Pendiente", "Navegar a la gestión de prestaciones.", "OK");
            // Aquí iría la navegación real:
            // if (Application.Current.MainPage is NavigationPage navigationPage)
            // {
            //     var prestacionesPage = _serviceProvider.GetService<PrestacionesPage>(); // Necesitas crear esta página y ViewModel
            //     await navigationPage.PushAsync(prestacionesPage);
            // }
        }

        [RelayCommand]
        private async Task GoToPagos()
        {
            await Application.Current.MainPage.DisplayAlert("Funcionalidad Pendiente", "Navegar a la gestión de pagos.", "OK");
            // Aquí iría la navegación real:
            // if (Application.Current.MainPage is NavigationPage navigationPage)
            // {
            //     var pagosPage = _serviceProvider.GetService<PagosPage>(); // Necesitas crear esta página y ViewModel
            //     await navigationPage.PushAsync(pagosPage);
            // }
        }
    }
}