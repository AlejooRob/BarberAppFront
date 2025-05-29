using BarberAppFront.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberAppFront.Services
{
    public interface IUsuarioService
    {
        // Endpoint para crear un usuario (por ADMIN), este endpoint requiere autenticación (el token del ADMIN)
        [Post("/")] // La URL base ya está configurada en MauiProgram.cs como /api/usuarios
        [Headers("Authorization: Bearer")] // Refit agrega el token 
        Task<ApiResponse<Usuario>> CreateUser([Body] Usuario usuario);

        // Otros endpoints de usuario, si los necesitas
        // [Get("/")]
        // [Headers("Authorization: Bearer")]
        // Task<ApiResponse<List<Usuario>>> GetUsers();
    }
}
