using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using WeatherReport.Models;

namespace WeatherReport.Controllers
{
    [ApiController]
    [Route("WeatherApi")] //Sets default routing to this controller as .../WeatherApi
    public class WeatherForecastController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly HttpClient _httpClient;
        private readonly WeatherApiSettings _apiSettings;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IHttpClientFactory httpClientFactory, IOptions<WeatherApiSettings> apiSettings)
        {
            _logger = logger;  //DInjected. Used to log status of application.
            _httpClient = httpClientFactory.CreateClient(); //DInjected httpClientFactory. Using factory design pattern to create object needed.
            _apiSettings = apiSettings.Value; //DInjected object representing api settings from appsettings.json
        }


        /* Returns the Weather of a place in a determined time.
         * GET call as .../WeatherAPI/GetWeather.
         * placeName: Defines a place. Could be the actual name of the place or latitude and longitude (e.g: 48.8567,2.3508).
         * date: Defines a date. (yyyy-mm-dd)
         * hour: Defines an hour. (0-23)
         */
        [HttpGet("GetWeather")]
        public async Task<IActionResult> GetWeather(string placeName, DateTime date, int hour)  //Return type is Task<IActionResult> and not IActionResult because the operation is async! Task handles mechanism of async such as concurrency.
        {
            string apiUrl = $"{_apiSettings.HistoryUrl}?key={_apiSettings.WeatherAPIKey}&q={placeName}&dt={date}&hour={hour}";
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            return Ok(response);
        }
    }
}