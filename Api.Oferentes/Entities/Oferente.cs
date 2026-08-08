namespace Api.Oferentes.Entities
{
    public class Oferente
    {
        public int IdOferente { get; set; }
        public string Identificacion { get; set; } = null!;
        public string TipoIdentificacion { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public DateTime FechaNacimiento { get; set; }
        public string? RutaCurriculum { get; set; }
    }
}