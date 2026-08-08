namespace Api.Oferentes.Entities
{
    // Usado en Core2: listado con solo nombre e identificación
    public class OferenteResumen
    {
        public string Identificacion { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
    }
}