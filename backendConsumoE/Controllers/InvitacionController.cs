using backendConsumoE.Dtos;
using backendConsumoE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backendConsumoE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvitacionController : ControllerBase
    {
        private readonly InvitacionService _service;
        public InvitacionController(InvitacionService service) => _service = service;


        //[Authorize]
        [HttpGet("usuario/por-correo/{correo}")]
        public async Task<IActionResult> ObtenerUsuarioPorCorreo(string correo)
        {
            try
            {
                var idUsuario = await _service.ObtenerIdUsuarioPorCorreoAsync(correo);
                return Ok(new { idUsuario });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { mensaje = "No existe ningún usuario con ese correo." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { mensaje = "Error interno al buscar usuario." });
            }
        }

        [HttpPost("enviar")]
        public async Task<IActionResult> EnviarInvitacion([FromBody] EnviarInvitacionDto dto)
        {
            try
            {
                string mensaje = await _service.EnviarInvitacionAsync(
                    dto.IdInvitador,
                    dto.IdInvitado,
                    dto.IdHogar,
                    dto.IdRol);

                if (mensaje == "Invitación enviada exitosamente.")
                    return Ok(new { mensaje });

                if (mensaje == "Ya existe una invitación pendiente.")
                    return Conflict(new { error = mensaje });

                // Cualquier otro mensaje de SP o validación
                return BadRequest(new { error = mensaje });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                // Ocultamos detalles internos
                return StatusCode(500, new { error = "Error interno al procesar la invitación." });
            }
        }

        [HttpGet("pendientes/invitado/{idInvitado}")]
        public async Task<IActionResult> ListarPendientesPorInvitado(int idInvitado)
        {
            try
            {
                var pendientes = await _service.ListarPendientesPorInvitadoAsync(idInvitado);
                return Ok(pendientes);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno al listar invitaciones pendientes." });
            }
        }

        [HttpPost("aceptar")]
        public async Task<IActionResult> Aceptar([FromBody] ProcesarInvitacionDto dto)
        {
            try
            {
                string msg = await _service.AceptarInvitacionAsync(dto.IdInvitacion);
                return Ok(new { mensaje = msg });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch
            {
                return StatusCode(500, new { error = "Error interno al aceptar invitación." });
            }
        }

        [HttpPost("rechazar")]
        public async Task<IActionResult> Rechazar([FromBody] ProcesarInvitacionDto dto)
        {
            try
            {
                string msg = await _service.RechazarInvitacionAsync(dto.IdInvitacion);
                return Ok(new { mensaje = msg });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch
            {
                return StatusCode(500, new { error = "Error interno al rechazar invitación." });
            }
        }

        [HttpPost("expirar")]
        public async Task<IActionResult> Expirar()
        {
            try
            {
                await _service.ExpirarInvitacionesAsync();
                return Ok(new { mensaje = "Expiraciones procesadas." });
            }
            catch
            {
                return StatusCode(500, new { error = "Error al expirar invitaciones." });
            }
        }

    }
}