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


        /// <summary>
        /// Obtiene el ID del usuario a partir de su correo electrónico.
        /// </summary>
        [Authorize]
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

        /// <summary>
        /// Envía una invitación para agregar un usuario a un hogar con un rol específico.
        /// </summary>
        [Authorize]
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

        /// <summary>
        /// Lista todas las invitaciones pendientes para un usuario invitado específico.
        /// </summary>
        [Authorize]
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

        /// <summary>
        /// Acepta una invitación pendiente para un hogar.
        /// </summary>
        [Authorize]
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

        /// <summary>
        /// Rechaza una invitación pendiente para un hogar.
        /// </summary>
        [Authorize]
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


        /// <summary>
        /// Marca como expiradas todas las invitaciones vencidas según la lógica definida.
        /// </summary>
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