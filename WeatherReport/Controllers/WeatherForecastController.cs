using Microsoft.AspNetCore.Mvc;
using WeatherReport.Models;

namespace WeatherReport.Controllers
{
    [ApiController]
    [Route("WeatherApi")] //Sets default routing to this controller as .../WeatherApi
    public class WeatherForecastController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;  //DInjected. Used to log status of application.
        }


        /* Returns the Weather of a place in a determined time.
         * GET call as .../WeatherAPI/GetWeather
         */
        [HttpGet("GetWeather")]
        public string GetWeather()
        {
            return "AAA";
        }
    }
}