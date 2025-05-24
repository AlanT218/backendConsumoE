using backendConsumoE.Dtos;
using backendConsumoE.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backendConsumoE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        
        /// <summary>
        /// Inicia sesión para un usuario mediante correo y contraseña.
        /// </summary>
        [HttpPost("InicioSesion")]
        [AllowAnonymous]
        public async Task<IActionResult> PostIniciarSesion([FromBody] RequestInicioSesionDto requestInicioSesionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var resultado = await _userService.InicioSesion(requestInicioSesionDto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Permite al usario registrarse en el sistema.
        /// </summary>
        [HttpPost("CrearUsuario")]
        [AllowAnonymous]
        public async Task<IActionResult> CrearUsuarios([FromBody] RequestUserDto requestUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                ResponseGeneralDto response = await _userService.CrearUsuario(requestUserDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}

//[Authorize]
//[HttpGet("ObtenerUsuarios")]
//public async Task<IActionResult> ObtenerUsuarios()
//{
//    try
//    {
//        var usuarios = await _userService.ObtenerUsuarios();
//        return Ok(usuarios);
//    }
//    catch (Exception ex)
//    {
//        return StatusCode(500, new { message = ex.Message });
//    }
//}

//[HttpPost("RegistrarUsuario")]
//public async Task<IActionResult> RegistrarUsuario([FromBody] UserDto usuario)
//{
//    if (usuario == null)
//        return BadRequest("Datos inválidos");

//    await _userService.RegistrarUsuario(usuario);
//    return Ok("Usuario registrado correctamente");
//}

//[HttpPost("Registrar")]
//public IActionResult CrearUsuario([FromBody] UserDto usuario)
//{
//    try
//    {
//        if (usuario == null)
//        {
//            return BadRequest("El usuario no puede ser nulo.");
//        }

//        _userService.CrearUsuario(usuario);

//        return Ok(new { mensaje = "Usuario registrado correctamente." });
//    }
//    catch (ArgumentException ex)
//    {
//        return BadRequest(new { error = ex.Message });
//    }
//    catch (Exception ex)
//    {
//        return StatusCode(500, new { error = "Error interno del servidor: " + ex.Message });
//    }
//}

//[HttpPost("login")]
//public async Task<IActionResult> Login([FromBody] UserDto usuario)
//{
//    try
//    {
//        if (usuario == null)
//        {
//            return BadRequest("El usuario no puede ser nulo.");
//        }

//        UserDto userResult = await _userService.Login(usuario);

//        return Ok(new { mensaje = userResult.Mensaje });
//    }
//    catch (ArgumentException ex)
//    {
//        return BadRequest(new { error = ex.Message });
//    }
//    catch (Exception ex)
//    {
//        return StatusCode(500, new { error = "Error interno del servidor: " + ex.Message });
//    }
//}


