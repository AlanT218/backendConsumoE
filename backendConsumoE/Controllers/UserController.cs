using backendConsumoE.Dtos;
using backendConsumoE.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backendConsumoE.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }
        [Authorize]
        [HttpGet("ObtenerUsuarios")]
        public async Task<IActionResult> ObtenerUsuarios()
        {
            var usuarios = await _userService.ObtenerUsuario();
            return Ok(usuarios);
        }

        [HttpPost]
        [Route("/api/[Controller]/InicioSesion")]
        [AllowAnonymous]
        public async Task<IActionResult> PostIniciarSesion([FromBody] RequestInicioSesionDto requestInicioSesionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(await _userService.InicioSesion(requestInicioSesionDto));
        }

        [HttpPost]
        [Route("/api/[Controller]/CrearUsuario")]
        [AllowAnonymous]
        public async Task<IActionResult> CrearUsuarios([FromBody] RequestUserDto requestUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ResponseGeneralDto responseGeneralDto = await _userService.CrearUsuario(requestUserDto);

            return Ok(responseGeneralDto);
        }

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



        // GET: UserController
        public ActionResult Index()
        {
            return View();
        }

        // GET: UserController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        
    

        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
