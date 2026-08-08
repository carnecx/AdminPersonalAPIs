using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Api.Oferentes.Entities
{
    public class OferentePostulacion
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "La identificación es requerida")]
        public string Identificacion { get; set; } = null!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "El tipo de identificación es requerido")]
        public string TipoIdentificacion { get; set; } = null!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "El nombre completo es requerido")]
        public string NombreCompleto { get; set; } = null!;

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        public DateTime FechaNacimiento { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe indicar al menos un correo")]
        public string Correos { get; set; } = null!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe indicar al menos un teléfono")]
        public string Telefonos { get; set; } = null!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "El código de puesto es requerido")]
        public string CodigoPuesto { get; set; } = null!;

        // El archivo va DENTRO de la misma clase, ya no como parámetro aparte
        public IFormFile? Curriculum { get; set; }
    }
}