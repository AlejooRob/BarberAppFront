using System.Text.Json.Serialization;

namespace BarberAppFront.Models
{
    public class Servicio
    {

        public long? Id { get; set; }
        public string Nombre { get; set; }
        public double Precio { get; set; } // O decimal si usas BigDecimal en backend
        public string Descripcion { get; set; }

        // Si tienes relaciones en el backend que no quieres traer al frontend:
        // [JsonIgnore] // Ignorar para evitar bucles de serialización
        // public List<PrestacionServicio> PrestacionesRegistradas { get; set; }
    }
}
