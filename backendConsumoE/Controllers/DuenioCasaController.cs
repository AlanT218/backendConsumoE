using backendConsumoE.Services;
using backendConsumoE.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        // LISTAR electrodomésticos por hogar
        [HttpGet("zona-electro/hogar/{idHogar}")]
        public async Task<IActionResult> ObtenerZonaElectPorHogar(int idHogar)
        {
            try
            {
                var lista = await _duenioCasaService.ObtenerZonaElectPorHogarAsync(idHogar);

                if (lista == null || !lista.Any())
                {
                    return Ok(new { mensaje = "No hay electrodomésticos registrados para este hogar.", datos = new List<ZonaElectDto>() });
                }

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener los electrodomésticos del hogar: " + ex.Message });
            }
        }

        // ACTUALIZAR electrodoméstico
        [HttpPut("zona-electro/{idZonaElectro}")]
        public async Task<IActionResult> ActualizarZonaElectro(int idZonaElectro, [FromBody] ZonaElectroActualizarDto dto)
        {
            try
            {
                if (idZonaElectro != dto.IdZonaElectro)
                {
                    return BadRequest(new { mensaje = "El ID en la ruta no coincide con el del cuerpo de la solicitud." });
                }

                var actualizado = await _duenioCasaService.ActualizarZonaElectroAsync(dto);

                if (actualizado)
                    return Ok(new { mensaje = "Electrodoméstico actualizado correctamente." });
                else
                    return NotFound(new { mensaje = "No se encontró el electrodoméstico para actualizar." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar el electrodoméstico: " + ex.Message });
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


    }
}
