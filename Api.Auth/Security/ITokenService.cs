using Api.Auth.Entities;

namespace Api.Auth.Security
{
    // define las operaciones necesarias para generar tokens
    public interface ITokenService
    {
        // genera un token jwt para el usuario autenticado
        string GenerarToken(
            Usuario usuario,
            string rol
        );
    }
}