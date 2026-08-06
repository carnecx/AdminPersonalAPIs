using System.ComponentModel.DataAnnotations;

namespace Api.Puestos.Entities
{
    public class Puesto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "El código del puesto es requerido")]
        public string Codigo { get; set; } = null!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "El nombre del puesto es requerido")]
        public string Nombre { get; set; } = null!;
    }
}