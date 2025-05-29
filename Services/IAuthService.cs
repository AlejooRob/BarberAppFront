using BarberAppFront.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberAppFront.Services
{
    public interface IAuthService
    {
        // Endpoint para el login
        [Post("/login")]
        Task<ApiResponse<JwtResponse>> Login([Body] LoginRequest request);

        // Se deja desabilitado endpoint para el registro (si el AuthController también lo maneja, aunque para Admin usaremos /api/usuarios)
        // [Post("/register")]
        // Task<ApiResponse<MessageResponse>> Register([Body] SignupRequest request);
    }
}
