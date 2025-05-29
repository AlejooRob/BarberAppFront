using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberAppFront.Models
{
    public class SignupRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string NombreCompleto { get; set; }
        // añadir un campo para el rol si el ADMIN lo va a asignar
        // public string Rol { get; set; }
        // public string Estado { get; set; }
    }
}
