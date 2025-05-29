using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberAppFront.Models
{
    public class JwtResponse
    {
        public string Token { get; set; }
        public string Type { get; set; } = "Bearer";
        public long Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; } 
        public string NombreCompleto { get; set; }
    }
}
