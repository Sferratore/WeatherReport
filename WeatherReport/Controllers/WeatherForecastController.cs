using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text.Json;
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
            //Writing request
            string apiUrl = $"{_apiSettings.HistoryUrl}?key={_apiSettings.WeatherAPIKey}&q={placeName}&dt={date}&hour={hour}";

            //Awaiting response from WeatherAPI
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

            //Creating response for current API
            WeatherForecast wf = new WeatherForecast();
            var jsonString = await response.Content.ReadAsStringAsync();
            using (var jsonDoc = JsonDocument.Parse(jsonString))
            {
                // Assume the JSON structure is like { "id": 123, "name": "John Doe", ... }
                JsonElement root = jsonDoc.RootElement;

                // Access specific fields
                wf.Country = root.GetProperty("location").GetProperty("country").GetString();
                wf.City = root.GetProperty("location").GetProperty("name").GetString();
                wf.TemperatureC = root.GetProperty("forecast").GetProperty("forecastday")[0].GetProperty("hour")[0].GetProperty("temp_c").GetDouble();
                wf.Date = root.GetProperty("forecast").GetProperty("forecastday")[0].GetProperty("hour")[0].GetProperty("time").GetString();

            }

            
            return Ok(wf);
        }
    }
}