using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BarberAppFront.Models
{
    public class Usuario
    {
        public long? Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string NombreCompleto { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RolUsuario Rol { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EstadoUsuario Estado { get; set; }

        // public List<PrestacionServicio> PrestacionesRealizadas { get; set; }
        // public List<Pago> PagosRecibidos { get; set; }
    }

    public enum RolUsuario
    {
        ADMIN,
        BARBERO
    }

    public enum EstadoUsuario
    {
        ACTIVO,
        INACTIVO
    }
}
