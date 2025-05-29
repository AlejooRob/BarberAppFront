using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BarberAppFront.Models;
using BarberAppFront.Services;
using BarberAppFront.Utils;
using BarberAppFront.Views; // Para navegación a AddEditPrestacionPage
using Refit; // Para ApiException
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls; // Para Application.Current.MainPage
using Microsoft.Maui; // Para Preferences

namespace BarberAppFront.ViewModels
{
    public partial class PrestacionesViewModel : ObservableObject
    {
        private readonly IPrestacionServicioService _prestacionServicioService;
        private readonly IServiceProvider _serviceProvider; // Para resolver páginas de navegación

        [ObservableProperty]
        private ObservableCollection<PrestacionServicio> prestaciones;

        [ObservableProperty]
        private PrestacionServicio selectedPrestacion; // Para selección de elemento

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage;

        [ObservableProperty]
        private bool hasError;

        public PrestacionesViewModel(IPrestacionServicioService prestacionServicioService, IServiceProvider serviceProvider)
        {
            _prestacionServicioService = prestacionServicioService;
            _serviceProvider = serviceProvider;
            Prestaciones = new ObservableCollection<PrestacionServicio>();
        }

        // Comando para cargar las prestaciones (se ejecuta al aparecer la página)
        [RelayCommand]
        private async Task LoadPrestaciones()
        {
            IsBusy = true;
            HasError = false;
            ErrorMessage = string.Empty;

            try
            {
                string userRole = Preferences.Get("userRole", string.Empty);
                string userIdString = Preferences.Get("userId", string.Empty);

                if (string.IsNullOrEmpty(userIdString))
                {
                    ErrorMessage = "ID de usuario no encontrado. Por favor, vuelva a iniciar sesión.";
                    HasError = true;
                    return;
                }

                long userId = long.Parse(userIdString);
                ApiResponse<List<PrestacionServicio>> response = null;

                if (userRole.Equals("ADMIN", StringComparison.OrdinalIgnoreCase))
                {
                    // Si es ADMIN, puede ver todas las prestaciones
                    response = await _prestacionServicioService.GetAll();
                }
                else if (userRole.Equals("BARBERO", StringComparison.OrdinalIgnoreCase))
                {
                    // Si es BARBERO, solo ve sus propias prestaciones
                    response = await _prestacionServicioService.GetByBarberoId(userId);
                }
                else
                {
                    ErrorMessage = "Rol de usuario no reconocido o insuficiente para ver prestaciones.";
                    HasError = true;
                    return;
                }

                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    Prestaciones.Clear();
                    foreach (var prestacion in response.Content)
                    {
                        Prestaciones.Add(prestacion);
                    }
                }
                else
                {
                    ErrorMessage = $"Error al cargar prestaciones: {response.StatusCode}. ";
                    if (response.Error?.Content != null)
                    {
                        try
                        {
                            MessageResponse errorParsed = System.Text.Json.JsonSerializer.Deserialize<MessageResponse>(response.Error.Content);
                            ErrorMessage += errorParsed?.Message ?? response.Error.Content;
                        }
                        catch { ErrorMessage += "Respuesta de error inesperada."; }
                    }
                    HasError = true;
                }
            }
            catch (ApiException ex)
            {
                ErrorMessage = $"Error de API: {ex.StatusCode}. Detalles: {ex.Content}";
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

        [RelayCommand]
        private async Task AddPrestacion()
        {
            if (Application.Current.MainPage is NavigationPage navigationPage)
            {
                var addEditPage = _serviceProvider.GetService<AddEditPrestacionPage>();
                if (addEditPage != null)
                {
                    // Al navegar a la página de añadir/editar, su ViewModel se inyectará automáticamente.
                    // Si necesitas inicializar algo en el ViewModel de AddEditPrestacionPage para una nueva creación,
                    // puedes añadir un método Initialize() a AddEditPrestacionViewModel y llamarlo así:
                    // ((AddEditPrestacionViewModel)addEditPage.BindingContext)?.Initialize(null); // null para nueva creación

                    await navigationPage.PushAsync(addEditPage);
                }
            }
        }

        // Comando para editar una prestación existente (navega a la página de edición/creación con datos)
        // [RelayCommand]
        // private async Task EditPrestacion(PrestacionServicio prestacion)
        // {
        //     if (prestacion == null) return;
        //     if (Application.Current.MainPage is NavigationPage navigationPage)
        //     {
        //         var addEditPage = _serviceProvider.GetService<AddEditPrestacionPage>();
        //         if (addEditPage != null)
        //         {
        //             // Asignar el BindingContext con la prestación existente
        //             // addEditPage.BindingContext = new AddEditPrestacionViewModel(_prestacionServicioService, _servicioService, _serviceProvider, prestacion);
        //             // Para que DI maneje: crear un constructor en AddEditPrestacionPage que acepte un PrestacionServicio
        //             // o usar un método Initialize() en el ViewModel después de navegar.
        //             await navigationPage.PushAsync(addEditPage);
        //         }
        //     }
        // }

        // Comando para eliminar una prestación (solo ADMIN)
        [RelayCommand]
        private async Task DeletePrestacion(PrestacionServicio prestacion)
        {
            // Lógica para confirmar y eliminar
            // Solo ADMIN puede eliminar según el backend
            string userRole = Preferences.Get("userRole", string.Empty);
            if (!userRole.Equals("ADMIN", StringComparison.OrdinalIgnoreCase))
            {
                await Application.Current.MainPage.DisplayAlert("Acceso Denegado", "Solo los administradores pueden eliminar prestaciones.", "OK");
                return;
            }

            bool confirm = await Application.Current.MainPage.DisplayAlert("Confirmar Eliminación", $"¿Está seguro de eliminar la prestación de {prestacion.Servicio.Nombre}?", "Sí", "No");
            if (confirm)
            {
                IsBusy = true;
                try
                {
                    var response = await _prestacionServicioService.Delete(prestacion.Id);
                    if (response.IsSuccessStatusCode)
                    {
                        Prestaciones.Remove(prestacion);
                        await Application.Current.MainPage.DisplayAlert("Éxito", "Prestación eliminada correctamente.", "OK");
                    }
                    else
                    {
                        ErrorMessage = $"Error al eliminar: {response.StatusCode}. Detalles: {response.Error?.Content}";
                        HasError = true;
                    }
                }
                catch (ApiException ex)
                {
                    ErrorMessage = $"Error de API al eliminar: {ex.StatusCode}. Detalles: {ex.Content}";
                    HasError = true;
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Ocurrió un error inesperado al eliminar: {ex.Message}";
                    HasError = true;
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }
    }
}
