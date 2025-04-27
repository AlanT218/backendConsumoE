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

    }
}
