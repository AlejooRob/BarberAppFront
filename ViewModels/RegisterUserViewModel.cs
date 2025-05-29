using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BarberAppFront.Models;
using BarberAppFront.Services;
using BarberAppFront.Utils;
using Refit;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace BarberAppFront.ViewModels
{
    public partial class RegisterUserViewModel : ObservableObject
    {
        private readonly IUsuarioService _usuarioService;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string nombreCompleto;

        [ObservableProperty]
        private RolUsuario selectedRole; // Asumiendo que RolUsuario es un enum
        public ObservableCollection<RolUsuario> Roles { get; } = new ObservableCollection<RolUsuario>(Enum.GetValues<RolUsuario>());

        [ObservableProperty]
        private EstadoUsuario selectedEstado; // Asumiendo que EstadoUsuario es un enum
        public ObservableCollection<EstadoUsuario> Estados { get; } = new ObservableCollection<EstadoUsuario>(Enum.GetValues<EstadoUsuario>());

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string message; // Para mensajes de éxito
        [ObservableProperty]
        private bool hasMessage;

        [ObservableProperty]
        private string errorMessage; // Para mensajes de error
        [ObservableProperty]
        private bool hasError;

        public RegisterUserViewModel(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [RelayCommand]
        private async Task RegisterUser()
        {
            IsBusy = true;
            HasMessage = false;
            Message = string.Empty;
            HasError = false;
            ErrorMessage = string.Empty;

            try
            {
                // ... (validaciones de campos vacíos) ...

                var newUser = new Usuario
                {
                    Username = Username,
                    Password = Password,
                    NombreCompleto = NombreCompleto,
                    Rol = SelectedRole,
                    Estado = SelectedEstado
                };

                // La llamada a CreateUser de IUsuarioService ya tiene [Headers("Authorization: Bearer")]
                var response = await _usuarioService.CreateUser(newUser);

                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    Message = "Usuario registrado exitosamente!";
                    HasMessage = true;
                    // Limpiar los campos después del registro exitoso
                    Username = string.Empty;
                    Password = string.Empty;
                    NombreCompleto = string.Empty;
                    SelectedRole = RolUsuario.BARBERO; // Reset a un valor por defecto
                    SelectedEstado = EstadoUsuario.ACTIVO; // Reset a un valor por defecto
                }
                else // Aquí se manejan los errores como 403 Forbidden, 400 Bad Request, 500 Internal Server Error, etc.
                {
                    ErrorMessage = $"Error al registrar. Código de estado: {response.StatusCode}. ";
                    HasError = true;

                    if (response.StatusCode == System.Net.HttpStatusCode.Forbidden) // Si es un 403
                    {
                        ErrorMessage += "Acceso denegado. Asegúrese de tener permisos de administrador.";
                    }
                    // Intenta leer el contenido del error del backend, esperando que sea un MessageResponse
                    else if (response.Error?.Content != null)
                    {
                        try
                        {
                            // Intentar deserializar como MessageResponse (si el backend lo envía así para errores)
                            MessageResponse errorParsed = System.Text.Json.JsonSerializer.Deserialize<MessageResponse>(response.Error.Content);
                            ErrorMessage += errorParsed?.Message ?? "Respuesta de error inesperada del servidor.";
                        }
                        catch (System.Text.Json.JsonException jsonEx) // Si el contenido no es JSON, captura la JsonException
                        {
                            ErrorMessage += $"Respuesta del servidor no es JSON o no es el esperado. Contenido: {response.Error.Content?.Substring(0, Math.Min(response.Error.Content.Length, 200))}...";
                            // Aquí puedes añadir un Debug.WriteLine para ver el JSONEx completo
                            System.Diagnostics.Debug.WriteLine($"Error de deserialización en el manejo de error: {jsonEx.Message}");
                        }
                        catch (Exception ex) // Otros errores al procesar el contenido de error
                        {
                            ErrorMessage += $"Error al procesar la respuesta de error: {ex.Message}";
                        }
                    }
                    else
                    {
                        ErrorMessage += "El servidor respondió con un error, pero sin detalles adicionales.";
                    }
                }
            }
            catch (ApiException ex) // Errores de Refit que no son 2xx, ya envueltos en la respuesta.Error
            {
                // Esta excepción ApiException no debería capturarse aquí si la respuesta es de tipo ApiResponse<T>
                // ya que IsSuccessStatusCode maneja eso. Esta sección es más para errores de red antes de recibir respuesta.
                ErrorMessage = $"Error de conectividad de API: {ex.Message}";
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