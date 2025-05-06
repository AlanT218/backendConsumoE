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

        //[Authorize]
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

        //[Authorize]
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

        //[Authorize]
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

        // ACTUALIZAR electrodoméstico
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

        // ELIMINAR electrodoméstico
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

        //[HttpPost("cambiar-estado")]
        //public async Task<IActionResult> CambiarEstado([FromBody] CambioEstadoDto dto)
        //{
        //    if (dto == null)
        //        return BadRequest("Datos inválidos");

        //    try
        //    {
        //        await _duenioCasaService.CambiarEstadoElectrodomesticoAsync(dto);
        //        return Ok(new { mensaje = "Estado actualizado correctamente." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { mensaje = ex.Message });
        //    }
        //}
        // POST: api/DuenioCasa/cambiar-estado
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
        // GET: api/DuenioCasa/estado-actual/{idZonaElect}
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

        [HttpGet("generar-reporte-pdf/{idHogar}")]
        public IActionResult GenerarReportePdf(int idHogar)
        {
            var datos = _duenioCasaService.ObtenerDatosReporteConsumo(idHogar);

            if (datos == null || !datos.Any())
                return NotFound("No se encontraron datos de consumo.");

            var pdfBytes = PdfGenerator.CreateReporteConsumoPdf(datos);

            return File(pdfBytes, "application/pdf", $"ReporteConsumo_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
        }
        

    }
}
