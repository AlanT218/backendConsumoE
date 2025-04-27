using backendConsumoE.Dtos;
using backendConsumoE.Repositories;
using backendConsumoE.Utilities;

namespace backendConsumoE.Services
{
    public class UserService
    {
        private readonly JwtSettingsDto _jwtSettings;
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository, JwtSettingsDto jwtSettings)
        {
            _userRepository = userRepository;
            _jwtSettings = jwtSettings;
        }

        public async Task<List<UserDto>> ObtenerUsuarios()
        {
            return await _userRepository.ObtenerUsuarios();
        }

        public async Task<ResponseInicionSesionDto> InicioSesion(RequestInicioSesionDto request)
        {
            var response = new ResponseInicionSesionDto();
            request.Contra = EncryptUtility.EncryptPassword(request.Contra);

            var user = await _userRepository.Login(request);

            if (user is not null && !string.IsNullOrWhiteSpace(user.Nombre))
            {
                response.IdUsuario = user.Id; // Agregas el ID antes de crear el token
                response = JwtUtility.GenTokenkey(response, _jwtSettings);
                response.Respuesta = 1;
                response.Mensaje = $"Inicio de sesión exitoso. Bienvenid@ {user.Nombre}";
            }
            else
            {
                response.Respuesta = 0;
                response.Mensaje = "Inicio de sesión fallido, correo y/o contraseña incorrectos.";
            }

            return response;
        }

        public async Task<ResponseGeneralDto> CrearUsuario(RequestUserDto requestUserDto)
        {
            var response = new ResponseGeneralDto();
            requestUserDto.Contra = EncryptUtility.EncryptPassword(requestUserDto.Contra);

            var filasAfectadas = await _userRepository.RegistrarUsuario(requestUserDto);

            response.Respuesta = filasAfectadas > 0 ? 1 : 0;
            response.Mensaje = filasAfectadas > 0
                ? "Usuario creado exitosamente."
                : "Algo pasó al registrar el usuario.";

            return response;
        }
    }
}



        //public void RegistrarUsuario(UserDto usuario)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(usuario.Nombre) || string.IsNullOrEmpty(usuario.Apellido) ||
        //            string.IsNullOrEmpty(usuario.Correo) || string.IsNullOrEmpty(usuario.Contra))
        //        {
        //            throw new ArgumentException("Todos los campos obligatorios deben ser proporcionados.");
        //        }

        //        _userRepository.RegistrarUsuario(usuario);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error en el servicio de usuario: " + ex.Message);
        //    }
        //}
        //public async Task<UserDto> Login(UserDto user)
        //{
        //    if (string.IsNullOrEmpty(user.Correo) || string.IsNullOrEmpty(user.Contra))
        //    {
        //        throw new ArgumentException("Correo y contraseña son obligatorios.");
        //    }

        //    return await _userRepository.Login(user);
        //}


