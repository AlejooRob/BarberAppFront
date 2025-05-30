using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BarberAppFront.Models;
using BarberAppFront.Services;
using BarberAppFront.Utils;
using Refit; // Para ApiException
using System;
using System.Collections.ObjectModel;
using System.Linq; // Para el método .FirstOrDefault()
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui; // Para Preferences

namespace BarberAppFront.ViewModels
{
    public partial class AddEditPrestacionViewModel : ObservableObject
    {
        private readonly IPrestacionServicioService _prestacionServicioService;
        private readonly IServicioService _servicioService;
        private readonly IUsuarioService _usuarioService; 
        private readonly IServiceProvider _serviceProvider; 

        [ObservableProperty]
        private PrestacionServicio currentPrestacion;

        [ObservableProperty]
        private ObservableCollection<Servicio> servicios;

        [ObservableProperty]
        private Servicio selectedService;

        [ObservableProperty]
        private ObservableCollection<EstadoPrestacion> estados; 
        [ObservableProperty]
        private EstadoPrestacion selectedEstado; 

       
        [ObservableProperty]
        private double precioReal;
        [ObservableProperty]
        private string observacion;

        [ObservableProperty]
        private bool isBusy;
        [ObservableProperty]
        private string errorMessage;
        [ObservableProperty]
        private bool hasError;
        [ObservableProperty]
        private string message;
        [ObservableProperty]
        private bool hasMessage;

        [ObservableProperty]
        private string pageTitle;

        // Constructor: inyecta los servicios necesarios
        public AddEditPrestacionViewModel(IPrestacionServicioService prestacionServicioService,
                                        IServicioService servicioService,
                                        IUsuarioService usuarioService,
                                        IServiceProvider serviceProvider)
        {
            _prestacionServicioService = prestacionServicioService;
            _servicioService = servicioService;
            _usuarioService = usuarioService;
            _serviceProvider = serviceProvider;

            Servicios = new ObservableCollection<Servicio>();
            Estados = new ObservableCollection<EstadoPrestacion>(Enum.GetValues<EstadoPrestacion>());
            PageTitle = "Registrar Nueva Prestación"; // Título por defecto

            // Inicializar la prestación actual para que el barbero y fecha se rellenen
            CurrentPrestacion = new PrestacionServicio();
            LoadInitialData(); // Cargar datos iniciales (barbero, servicios)
        }

        // Método para cargar datos iniciales (ej. barbero logueado y lista de servicios)
        private async Task LoadInitialData()
        {
            IsBusy = true;
            HasError = false;
            ErrorMessage = string.Empty;

            try
            {
                // Rellenar el barbero automáticamente (quien está logueado)
                string userIdString = Preferences.Get("userId", string.Empty);
                if (!string.IsNullOrEmpty(userIdString))
                {
                    long userId = long.Parse(userIdString);
                    
                    CurrentPrestacion.Barbero = new Usuario { Id = userId, NombreCompleto = Preferences.Get("userName", "Barbero Desconocido") };
                    // Guardar nombre de usuario también en preferences al loguearse para mostrarlo
                }
                else
                {
                    ErrorMessage = "No se pudo obtener el ID del barbero logueado.";
                    HasError = true;
                }

                CurrentPrestacion.FechaHora = DateTime.Now; // Fecha y hora actual

                // Cargar la lista de servicios disponibles
                var serviciosResponse = await _servicioService.GetAll();
                if (serviciosResponse.IsSuccessStatusCode && serviciosResponse.Content != null)
                {
                    Servicios.Clear();
                    foreach (var s in serviciosResponse.Content)
                    {
                        Servicios.Add(s);
                    }
                }
                else
                {
                    ErrorMessage = $"Error al cargar servicios: {serviciosResponse.StatusCode}. Detalles: {serviciosResponse.Error?.Content}";
                    HasError = true;
                }

                // Establecer estado por defecto (si es nueva)
                SelectedEstado = EstadoPrestacion.REALIZADO; // O PENDIENTE, según tu lógica de negocio
            }
            catch (ApiException ex)
            {
                ErrorMessage = $"Error de API al cargar datos iniciales: {ex.StatusCode}. Detalles: {ex.Content}";
                HasError = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ocurrió un error inesperado al cargar datos iniciales: {ex.Message}";
                HasError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Método para inicializar para edición (si se pasa una prestación existente)
        public void Initialize(PrestacionServicio prestacionToEdit)
        {
            if (prestacionToEdit != null)
            {
                PageTitle = "Editar Prestación Existente";
                CurrentPrestacion = prestacionToEdit; // Asignar la prestación para edición
                SelectedService = Servicios.FirstOrDefault(s => s.Id == prestacionToEdit.Servicio?.Id);
                SelectedEstado = prestacionToEdit.Estado;
                PrecioReal = prestacionToEdit.PrecioReal;
                Observacion = prestacionToEdit.Observacion;
            }
            else
            {
                PageTitle = "Registrar Nueva Prestación";
                CurrentPrestacion = new PrestacionServicio();
                LoadInitialData(); // Si es nueva, cargar datos iniciales
            }
        }


        // Comando para guardar la prestación (crear o actualizar)
        [RelayCommand]
        private async Task SavePrestacion()
        {
            IsBusy = true;
            HasMessage = false;
            Message = string.Empty;
            HasError = false;
            ErrorMessage = string.Empty;

            try
            {
                // Validaciones básicas de UI
                if (SelectedService == null)
                {
                    ErrorMessage = "Por favor, selecciona un servicio.";
                    HasError = true;
                    return;
                }
                if (PrecioReal <= 0)
                {
                    ErrorMessage = "El precio real debe ser mayor a cero.";
                    HasError = true;
                    return;
                }

                // Rellenar la prestación con los datos del formulario
                CurrentPrestacion.Servicio = SelectedService; // Objeto completo, Refit lo serializará con ID
                CurrentPrestacion.PrecioReal = PrecioReal;
                CurrentPrestacion.Observacion = Observacion;
                CurrentPrestacion.Estado = SelectedEstado;

                // Si CurrentPrestacion.Id es 0 (para long) o null (para long?), es una nueva prestación
                ApiResponse<PrestacionServicio> response;
                if (CurrentPrestacion.Id == 0 || CurrentPrestacion.Id == null)
                {
                    // Crear nueva prestación
                    response = await _prestacionServicioService.Register(CurrentPrestacion);
                }
                else
                {
                    // Actualizar prestación existente
                    response = await _prestacionServicioService.Update(CurrentPrestacion.Id, CurrentPrestacion);
                }

                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    Message = CurrentPrestacion.Id == 0 || CurrentPrestacion.Id == null ? "Prestación registrada exitosamente." : "Prestación actualizada exitosamente.";
                    HasMessage = true;

                    // Limpiar formulario o navegar de vuelta
                    // Para navegar de vuelta, usar Shell.Current.Navigation.PopAsync() o Application.Current.MainPage
                    if (Application.Current.MainPage is NavigationPage navigationPage)
                    {
                        await navigationPage.PopAsync(); // Vuelve a la lista de prestaciones
                    }
                }
                else
                {
                    ErrorMessage = $"Error al guardar prestación: {response.StatusCode}. ";
                    if (response.Error?.Content != null)
                    {
                        try
                        {
                            MessageResponse errorParsed = System.Text.Json.JsonSerializer.Deserialize<MessageResponse>(response.Error.Content);
                            ErrorMessage += errorParsed?.Message ?? "Respuesta de error inesperada.";
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
    }
}
