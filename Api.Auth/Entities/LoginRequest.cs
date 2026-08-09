namespace Api.Auth.Entities
{
    // representa los datos enviados para iniciar sesion
    public class LoginRequest
    {
        // usuario digitado en el login
        public string Usuario { get; set; } = string.Empty;

        // contrasena digitada en el login
        public string Contrasena { get; set; } = string.Empty;
    }
}