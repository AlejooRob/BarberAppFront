using Refit;
using BarberAppFront.Models;
//using BarberAppFront.Payload.Request;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BarberAppFront.Services
{
    public interface IPrestacionServicioService
    {
        
        [Post("/")]
        [Headers("Authorization: Bearer")]
        Task<ApiResponse<PrestacionServicio>> Register([Body] PrestacionServicio prestacion);

        [Get("/barbero/{barberoId}")]
        [Headers("Authorization: Bearer")]
        Task<ApiResponse<List<PrestacionServicio>>> GetByBarberoId(long barberoId);

        [Get("/")]
        [Headers("Authorization: Bearer")]
        Task<ApiResponse<List<PrestacionServicio>>> GetAll();

        [Get("/{id}")]
        [Headers("Authorization: Bearer")]
        Task<ApiResponse<PrestacionServicio>> GetById(long id);

        [Put("/{id}")]
        [Headers("Authorization: Bearer")]
        Task<ApiResponse<PrestacionServicio>> Update(long id, [Body] PrestacionServicio prestacion);

        [Delete("/{id}")]
        [Headers("Authorization: Bearer")]
        Task<ApiResponse<object>> Delete(long id); // Usamos object si no hay contenido específico de retorno
    }
}