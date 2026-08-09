using Api.Auth.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Auth.Security
{
    // servicio encargado de generar los tokens jwt
    public class TokenService : ITokenService
    {
        // permite leer los valores del appsettings
        private readonly IConfiguration _configuration;

        // constructor
        public TokenService(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // genera el token del usuario autenticado
        public string GenerarToken(
            Usuario usuario,
            string rol)
        {
            // obtiene la clave configurada para jwt
            string clave =
                _configuration["Jwt:Key"]
                ?? throw new Exception(
                    "No se encontro la clave JWT."
                );

            // crea los datos que iran dentro del token
            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    usuario.IdUsuario.ToString()
                ),

                new Claim(
                    ClaimTypes.Name,
                    usuario.NombreUsuario
                ),

                new Claim(
                    ClaimTypes.Role,
                    rol
                )
            };

            // convierte la clave a bytes
            var llave =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(clave)
                );

            // crea las credenciales para firmar el token
            var credenciales =
                new SigningCredentials(
                    llave,
                    SecurityAlgorithms.HmacSha256
                );

            // crea el token
            var token =
                new JwtSecurityToken(
                    issuer:
                        _configuration["Jwt:Issuer"],

                    audience:
                        _configuration["Jwt:Audience"],

                    claims:
                        claims,

                    expires:
                        DateTime.UtcNow.AddMinutes(60),

                    signingCredentials:
                        credenciales
                );

            // convierte el token a texto
            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}