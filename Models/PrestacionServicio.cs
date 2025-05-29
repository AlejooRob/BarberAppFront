using System;
using System.Text.Json.Serialization;

namespace BarberAppFront.Models
{
    public class PrestacionServicio
    {
        public long Id { get; set; }

        public Servicio Servicio { get; set; }

        // Referenciamos al Barbero (Usuario) por su ID o el objeto completo simplificado
        public Usuario Barbero { get; set; } // El Usuario tiene el Id, Username, etc.

        public DateTime FechaHora { get; set; } // Usar DateTime para LocalDateTime
        public double PrecioReal { get; set; }
        public string Observacion { get; set; }

        // Enum para el estado de la prestación
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EstadoPrestacion Estado { get; set; }

        public string ImagenUrl { get; set; }
    }

    // Asegúrate de que este enum coincida exactamente con tu backend
    public enum EstadoPrestacion
    {
        REALIZADO,
        CANCELADO,
        PENDIENTE
    }
}