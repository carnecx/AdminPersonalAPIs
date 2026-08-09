using Api.Auth.Entities;
using Api.Auth.Services;
using System.Security.Claims;

namespace Api.Auth
{
    // contiene los endpoints relacionados con autenticacion
    public static class AuthEndpoint
    {
        // registra los endpoints de autenticacion
        public static void MapAuthEndpoints(
            this WebApplication app)
        {
            // crea el grupo principal de autenticacion
            var group =
                app.MapGroup("/api/auth");

            // endpoint publico para iniciar sesion
            group.MapPost(
                "/login",
                async (
                    LoginRequest request,
                    AuthService authService) =>
                {
                    // ejecuta el proceso de autenticacion
                    LoginResponse respuesta =
                        await authService.LoginAsync(
                            request
                        );

                    // si la autenticacion fue correcta
                    if (respuesta.Exito)
                    {
                        return Results.Ok(respuesta);
                    }

                    // si el usuario esta bloqueado
                    if (respuesta.Mensaje.Contains(
                            "bloqueado",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        return Results.Json(
                            respuesta,
                            statusCode:
                                StatusCodes.Status403Forbidden
                        );
                    }

                    // si faltan datos
                    if (respuesta.Mensaje.Contains(
                            "Debe ingresar",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        return Results.BadRequest(
                            respuesta
                        );
                    }

                    // credenciales incorrectas
                    return Results.Json(
                        respuesta,
                        statusCode:
                            StatusCodes.Status401Unauthorized
                    );
                }
            );

            // endpoint protegido para comprobar el token
            group.MapGet(
                "/validar",
                (ClaimsPrincipal usuario) =>
                {
                    // obtiene el id guardado dentro del token
                    string? idUsuario =
                        usuario.FindFirstValue(
                            ClaimTypes.NameIdentifier
                        );

                    // obtiene el nombre de usuario del token
                    string? nombreUsuario =
                        usuario.FindFirstValue(
                            ClaimTypes.Name
                        );

                    // obtiene el rol del token
                    string? rol =
                        usuario.FindFirstValue(
                            ClaimTypes.Role
                        );

                    // devuelve los datos del usuario autenticado
                    return Results.Ok(
                        new
                        {
                            exito = true,
                            mensaje =
                                "Token válido. Acceso autorizado.",
                            idUsuario,
                            nombreUsuario,
                            rol
                        }
                    );
                }
            )
            // obliga a enviar un jwt valido
            .RequireAuthorization();
        }
    }
}