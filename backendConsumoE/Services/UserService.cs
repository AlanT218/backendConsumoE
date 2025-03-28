using backendConsumoE.Dtos;
using backendConsumoE.Repositories;

namespace backendConsumoE.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserDto>> ObtenerUsuario()
        {
            return await _userRepository.ObtenerUsuarios();
        }
        //public async Task RegistrarUsuario(UserDto usuario)
        //{
        //    await _userRepository.RegistrarUsuario(usuario);
        //}
        public void RegistrarUsuario(UserDto usuario)
        {
            try
            {
                if (string.IsNullOrEmpty(usuario.Nombre) || string.IsNullOrEmpty(usuario.Apellido) ||
                    string.IsNullOrEmpty(usuario.Correo) || string.IsNullOrEmpty(usuario.Contra))
                {
                    throw new ArgumentException("Todos los campos obligatorios deben ser proporcionados.");
                }

                _userRepository.RegistrarUsuario(usuario);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en el servicio de usuario: " + ex.Message);
            }
        }
        public async Task<UserDto> Login(UserDto user)
        {
            if (string.IsNullOrEmpty(user.Correo) || string.IsNullOrEmpty(user.Contra))
            {
                throw new ArgumentException("Correo y contraseña son obligatorios.");
            }

            return await _userRepository.Login(user);
        }

    }
}

