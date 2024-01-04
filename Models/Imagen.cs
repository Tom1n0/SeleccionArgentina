using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationSeleccionArgentina.Models
{
    public class Imagen
    {
        public int IdImagen { get; set; }
        public string? Nombre { get; set; }
        public string? Image { get; set; }
        public int? Edad { get; set; }
        public string? Equipo { get; set; }
        public decimal? Altura { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }
    }
}
