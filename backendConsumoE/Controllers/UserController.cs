using backendConsumoE.Dtos;
using backendConsumoE.Services;
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

        [HttpGet("ObtenerUsuarios")]
        public async Task<IActionResult> ObtenerUsuarios()
        {
            var usuarios = await _userService.ObtenerUsuario();
            return Ok(usuarios);
        }

        //[HttpPost("RegistrarUsuario")]
        //public async Task<IActionResult> RegistrarUsuario([FromBody] UserDto usuario)
        //{
        //    if (usuario == null)
        //        return BadRequest("Datos inválidos");

        //    await _userService.RegistrarUsuario(usuario);
        //    return Ok("Usuario registrado correctamente");
        //}

        [HttpPost("RegistrarDueñoCasa")]
        public IActionResult RegistrarUsuario([FromBody] UserDto usuario)
        {
            try
            {
                if (usuario == null)
                {
                    return BadRequest("El usuario no puede ser nulo.");
                }

                _userService.RegistrarUsuario(usuario);

                return Ok(new { mensaje = "Usuario registrado correctamente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor: " + ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto usuario)
        {
            try
            {
                if (usuario == null)
                {
                    return BadRequest("El usuario no puede ser nulo.");
                }

                UserDto userResult = await _userService.Login(usuario);

                return Ok(new { mensaje = userResult.Mensaje });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor: " + ex.Message });
            }
        }



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
