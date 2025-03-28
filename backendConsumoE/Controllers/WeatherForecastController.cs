using backendConsumoE.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace backendConsumoE.Controllers
{
   
    [ApiController]
    [Route("/api/[Controller]/Estado")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        //Se pueden capturar los datos de Iloger para  la base de datos
        //Almacena los errores
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("/api/[Controller]/EstadoApi")]
        public async Task<IActionResult> EstadoApi()
        {
            ResponseGeneralDto responseGeneralDto = new()
            {
                Respuesta = 200,
                Mensaje = "API EN EJECUCION CORRECTA"
            };
            return Ok(responseGeneralDto);
        }

        //Agrega informacion a un array con temperaturas aleatorias 
        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Post()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
