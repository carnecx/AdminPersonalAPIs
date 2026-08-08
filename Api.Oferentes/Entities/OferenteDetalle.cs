namespace Api.Oferentes.Entities
{
    // Usado en Core8: detalle completo de un oferente
    public class OferenteDetalle
    {
        public int IdOferente { get; set; }
        public string Identificacion { get; set; } = null!;
        public string TipoIdentificacion { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public DateTime FechaNacimiento { get; set; }
        public string? RutaCurriculum { get; set; }
        public List<string> Correos { get; set; } = new();
        public List<string> Telefonos { get; set; } = new();
    }
}