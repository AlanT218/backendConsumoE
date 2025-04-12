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

        public async Task<List<UserDto>> ObtenerUsuario()
        {
            return await _userRepository.ObtenerUsuarios();
        }
        public async Task<ResponseInicionSesionDto> InicioSesion(RequestInicioSesionDto requestInicioSesionDto)
        {
            ResponseInicionSesionDto responseDto = new();

            requestInicioSesionDto.Contra = EncryptUtility.EncryptPassword(requestInicioSesionDto.Contra);

            // Creamos una conexión directa sin convertir el DTO
            var user = await _userRepository.Login(requestInicioSesionDto); // Este método debe aceptar RequestInicioSesionDto

            if (user != null && !string.IsNullOrEmpty(user.Nombre))
            {
                responseDto = JwtUtility.GenTokenkey(responseDto, _jwtSettings);
                responseDto.Respuesta = 1;
                responseDto.Mensaje = $"Inicio de sesión exitoso. Bienvenid@ {user.Nombre}";
            }
            else
            {
                responseDto.Respuesta = 0;
                responseDto.Mensaje = "Inicio de sesión fallido, correo y/o contraseña incorrectos.";
            }

            return responseDto;
        }


        public async Task<ResponseGeneralDto> CrearUsuario(RequestUserDto requestUserDto)
        {
            ResponseGeneralDto responseGeneralDto = new();

            // Encriptar la contraseña antes de guardar
            requestUserDto.Contra = EncryptUtility.EncryptPassword(requestUserDto.Contra);

            // Usar la instancia del repositorio (_userRepository), no la clase directamente
            var result = await _userRepository.RegistrarUsuario(requestUserDto);

            if (result == 1)
            {
                responseGeneralDto.Respuesta = 1;
                responseGeneralDto.Mensaje = "Usuario creado exitosamente.";
            }
            else
            {
                responseGeneralDto.Respuesta = 0;
                responseGeneralDto.Mensaje = "Algo pasó al registrar el usuario.";
            }

            return responseGeneralDto;
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

    }
}

