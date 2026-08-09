namespace Api.Auth.Entities
{
    // representa la respuesta del inicio de sesion
    public class LoginResponse
    {
        // indica si el inicio de sesion fue correcto
        public bool Exito { get; set; }

        // mensaje devuelto por la api
        public string Mensaje { get; set; } = string.Empty;

        // token generado cuando el usuario inicia sesion
        public string? Token { get; set; }

        // identificador del usuario autenticado
        public int? IdUsuario { get; set; }

        // nombre completo del usuario
        public string? NombreCompleto { get; set; }

        // rol del usuario
        public string? Rol { get; set; }
    }
}