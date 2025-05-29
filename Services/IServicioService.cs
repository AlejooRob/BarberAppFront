using Refit;
using BarberAppFront.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BarberAppFront.Services
{
    public interface IServicioService
    {
        [Get("/")]
        [Headers("Authorization: Bearer")] // Lo añadimos por si acaso requiere autenticación
        Task<ApiResponse<List<Servicio>>> GetAll();

        // Puedes añadir otros métodos si necesitas (ej. GetById, Create, Update, Delete)
        // pero para seleccionar un servicio para una prestación, GetAll es suficiente por ahora.
    }
}