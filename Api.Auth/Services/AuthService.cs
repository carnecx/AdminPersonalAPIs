using Api.Auth.Entities;
using Api.Auth.Repository;
using Api.Auth.Security;

namespace Api.Auth.Services
{
    // servicio encargado de realizar el proceso de autenticacion
    public class AuthService
    {
        // repositorio utilizado para consultar usuarios
        private readonly UsuarioRepository _usuarioRepository;

        // repositorio utilizado para registrar acciones en bitacora
        private readonly BitacoraRepository _bitacoraRepository;

        // servicio utilizado para validar contrasenas
        private readonly PasswordService _passwordService;

        // servicio utilizado para generar el token jwt
        private readonly ITokenService _tokenService;

        // constructor del servicio
        public AuthService(
            UsuarioRepository usuarioRepository,
            BitacoraRepository bitacoraRepository,
            PasswordService passwordService,
            ITokenService tokenService)
        {
            _usuarioRepository = usuarioRepository;
            _bitacoraRepository = bitacoraRepository;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        // realiza el proceso completo de inicio de sesion
        public async Task<LoginResponse> LoginAsync(
            LoginRequest request)
        {
            // valida que se haya enviado usuario y contrasena
            if (string.IsNullOrWhiteSpace(request.Usuario) ||
                string.IsNullOrWhiteSpace(request.Contrasena))
            {
                return new LoginResponse
                {
                    Exito = false,
                    Mensaje = "Debe ingresar usuario y contraseña."
                };
            }

            // busca el usuario en la base de datos
            Usuario? usuario =
                await _usuarioRepository
                    .BuscarPorUsuarioAsync(request.Usuario);

            // valida que el usuario exista
            if (usuario == null)
            {
                return new LoginResponse
                {
                    Exito = false,
                    Mensaje = "Usuario y/o contraseña incorrectos."
                };
            }

            // valida si el usuario se encuentra bloqueado
            if (usuario.Estado.Equals(
                    "Bloqueado",
                    StringComparison.OrdinalIgnoreCase))
            {
                await _bitacoraRepository.RegistrarAsync(
                    usuario.IdUsuario,
                    "Intento de inicio de sesión con usuario bloqueado."
                );

                return new LoginResponse
                {
                    Exito = false,
                    Mensaje = "El usuario se encuentra bloqueado."
                };
            }

            // valida la contrasena
            bool contrasenaCorrecta =
                _passwordService.Validar(
                    request.Contrasena,
                    usuario.Contrasena
                );

            // si la contrasena es incorrecta registra el intento
            if (!contrasenaCorrecta)
            {
                int intentos =
                    await _usuarioRepository
                        .RegistrarIntentoFallidoAsync(usuario);

                await _bitacoraRepository.RegistrarAsync(
                    usuario.IdUsuario,
                    "Intento de inicio de sesión incorrecto."
                );

                // si llego a tres intentos informa que fue bloqueado
                if (intentos >= 3)
                {
                    return new LoginResponse
                    {
                        Exito = false,
                        Mensaje =
                            "Usuario bloqueado por exceder los intentos permitidos."
                    };
                }

                return new LoginResponse
                {
                    Exito = false,
                    Mensaje = "Usuario y/o contraseña incorrectos."
                };
            }

            // reinicia los intentos fallidos despues de autenticarse
            await _usuarioRepository
                .ReiniciarIntentosAsync(usuario.IdUsuario);

            // obtiene el rol del usuario
            string? rol =
                await _usuarioRepository
                    .ObtenerRolAsync(usuario.IdUsuario);

            // si no tiene rol utiliza un valor por defecto
            rol ??= "Sin rol";

            // genera el token jwt
            string token =
                _tokenService.GenerarToken(
                    usuario,
                    rol
                );

            // registra el inicio de sesion correcto
            await _bitacoraRepository.RegistrarAsync(
                usuario.IdUsuario,
                "Inicio de sesión realizado correctamente."
            );

            // devuelve la informacion al cliente
            return new LoginResponse
            {
                Exito = true,
                Mensaje = "Autenticación realizada correctamente.",
                Token = token,
                IdUsuario = usuario.IdUsuario,
                NombreCompleto = usuario.NombreCompleto,
                Rol = rol
            };
        }
    }
}