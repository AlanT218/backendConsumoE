using backendConsumoE.Services;
using backendConsumoE.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using backendConsumoE.Utilities;

namespace backendConsumoE.Controllers
{
        [ApiController]
    [Route("api/[controller]")]
    public class DuenioCasaController : ControllerBase
    {
        private readonly DuenioCasaService _duenioCasaService;

        public DuenioCasaController(DuenioCasaService duenioCasaService)
        {
            _duenioCasaService = duenioCasaService;
        }

        /// <summary>
        /// Obtiene la lista de tipos de hogar disponibles para registrar un hogar.
        /// </summary>
        [Authorize]
        [HttpGet("tipos-hogar")]
        public async Task<IActionResult> ObtenerTiposHogar()
        {
            try
            {
                var tipos = await _duenioCasaService.ObtenerTiposHogar();
                return Ok(tipos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener tipos de hogar: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene la lista de tipos de hogar disponibles para registrar un hogar.
        /// </summary>
        [Authorize]
        [HttpPost("registrar-hogar")]
        public async Task<IActionResult> RegistrarHogar([FromBody] HogarDto dto)
        {
            try
            {
                // 1) obtenemos el claim NameIdentifier (configurado en tu JWT como ClaimTypes.NameIdentifier)
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                    return Unauthorized(new { mensaje = "Token inválido o sin Id de usuario." });

                // 2) asignamos el IdUsuario al DTO
                dto.IdUsuario = userId;

                // 3) llamamos al servicio
                var resultado = await _duenioCasaService.RegistrarHogar(dto);
                if (resultado)
                    return Ok(new { mensaje = "Hogar registrado correctamente"});
                else
                    return BadRequest(new { mensaje = "No se pudo registrar el hogar." });
            }
            catch (ArgumentException ex)
            {
                // validaciones de dto.Nombre o dto.IdTipo
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todos los hogares registrados por el usuario autenticado.
        /// </summary>
        [Authorize]
        [HttpGet("Hogares")]
        public async Task<IActionResult> ObtenerHogares()
        {
            try
            {
                var idUsuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (idUsuarioClaim == null)
                    return Unauthorized(new { message = "Token inválido o sin ID de usuario." });

                int idUsuario = int.Parse(idUsuarioClaim.Value);
                var hogares = await _duenioCasaService.ObtenerHogaresPorUsuario(idUsuario);
                return Ok(hogares);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene la lista de zonas disponibles en el sistema.
        /// </summary>
        [Authorize]
        [HttpGet("zonas")]
        public async Task<IActionResult> ObtenerZonas()
        {
            try
            {
                var zonas = await _duenioCasaService.ObtenerZonasAsync();
                return Ok(zonas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Devuelve la lista de electrodomésticos registrados por el dueño de casa.
        /// </summary>
        [Authorize]
        [HttpGet("electrodomesticos")]
        public async Task<IActionResult> ObtenerElectrodomesticos()
        {
            try
            {
                var electrodomesticos = await _duenioCasaService.ObtenerElectrodomesticosAsync();
                return Ok(electrodomesticos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Agrega un nuevo electrodoméstico a una zona específica.
        /// </summary>
        [Authorize]
        [HttpPost("zona-electro")]
        public async Task<IActionResult> AgregarZonaElectro([FromBody] ZonaElectDto nuevaZonaElect)
        {
            try
            {
                await _duenioCasaService.AgregarZonaElectroAsync(nuevaZonaElect);
                return Ok(new { mensaje = "Electrodoméstico agregado correctamente a la zona." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene los electrodomésticos asignados a un hogar específico.
        /// </summary>
        [Authorize]
        [HttpGet("zona-electro/hogar/{idHogar}")]
        public async Task<IActionResult> ObtenerZonaElectPorHogar(int idHogar)
        {
            try
            {
                var resultado = await _duenioCasaService.ObtenerZonaElectPorHogarAsync(idHogar);
                if (resultado == null || !resultado.Any())
                {
                    return NotFound(new { mensaje = "No hay electrodomésticos registrados para este hogar." });
                }
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza los datos de un electrodoméstico asignado a una zona.
        /// </summary>
        [Authorize]
        [HttpPut("zona-electro/{idZonaElect}")]
        public async Task<IActionResult> ActualizarZonaElectro(int idZonaElect, [FromBody] ZonaElectroActualizarDto dto)
        {
            try
            {
                await _duenioCasaService.ActualizarZonaElectroAsync(idZonaElect, dto);
                return Ok(new { mensaje = "Registro actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Cambia el estado de un electrodoméstico de una zona específica a inactivo.
        /// </summary>
        [Authorize]
        [HttpDelete("zona-electro/{idZonaElect}")]
        public async Task<IActionResult> EliminarZonaElect(int idZonaElect)
        {
            try
            {
                var eliminado = await _duenioCasaService.EliminarZonaElectAsync(idZonaElect);

                if (eliminado)
                    return Ok(new { mensaje = "Electrodoméstico eliminado correctamente." });
                else
                    return NotFound(new { mensaje = "No se encontró el electrodoméstico a eliminar." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el electrodoméstico: " + ex.Message });
            }
        }

        /// <summary>
        /// Cambia el estado (encendido/apagado) de un electrodoméstico.
        /// </summary>
        [Authorize]
        [HttpPost("cambiar-estado")]
        public async Task<IActionResult> CambiarEstado([FromBody] CambioEstadoDto dto)
        {
            if (dto == null)
                return BadRequest("Datos inválidos");

            try
            {
                // Obtener el idUsuario desde el token (JWT)
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized("No se pudo identificar al usuario.");

                dto.IdUsuario = int.Parse(userIdClaim);

                await _duenioCasaService.CambiarEstadoElectrodomesticoAsync(dto);
                return Ok(new { mensaje = "Estado actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el estado actual (encendido o apagado) de un electrodoméstico asignado a una zona.
        /// </summary>
        [Authorize]
        [HttpGet("estado-actual/{idZonaElect}")]
        public async Task<IActionResult> ObtenerEstadoActual(int idZonaElect)
        {
            try
            {
                var estado = await _duenioCasaService.ObtenerEstadoZonaElectAsync(idZonaElect);
                return Ok(new { estado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Genera un reporte en PDF con el consumo energético de un hogar.
        /// </summary>
        [Authorize]
        [HttpGet("generar-reporte-pdf/{idHogar}")]
        public IActionResult GenerarReportePdf(int idHogar)
        {
            var datos = _duenioCasaService.ObtenerDatosReporteConsumo(idHogar);

            if (datos == null || !datos.Any())
                return NotFound("No se encontraron datos de consumo.");

            var pdfBytes = PdfGenerator.CreateReporteConsumoPdf(datos);

            return File(pdfBytes, "application/pdf", $"ReporteConsumo_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
        }

        /// <summary>
        /// Devuelve la lista de recomendaciones energéticas disponibles.
        /// </summary>
        [Authorize]
        [HttpGet("recomendaciones")]
        public async Task<IActionResult> ObtenerTodasRecomendaciones()
        {
            try
            {
                var lista = await _duenioCasaService.ObtenerTodasRecomendacionesAsync();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }
    }
}
