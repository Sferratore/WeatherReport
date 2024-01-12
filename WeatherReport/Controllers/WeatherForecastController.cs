using Microsoft.AspNetCore.Mvc;
using WeatherReport.Models;

namespace WeatherReport.Controllers
{
    [ApiController]
    [Route("WeatherApi")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeather")]
        public string GetWeather()
        {
            return "AAAAAAAA";
        }
    }
}