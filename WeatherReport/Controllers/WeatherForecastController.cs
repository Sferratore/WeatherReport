using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
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
        public async Task<IActionResult> GetWeather(string placeName, string date, int hour)  //Return type is Task<IActionResult> and not IActionResult because the operation is async! Task handles mechanism of async such as concurrency.
        {
            //Writing request
            string apiUrl = $"{_apiSettings.HistoryUrl}?key={_apiSettings.WeatherAPIKey}&q={placeName}&dt={date}&hour={hour}";

            //Awaiting response from WeatherAPI
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

            //Creating response for current API
            string jsonString = await response.Content.ReadAsStringAsync();

            //Giving back error in case something goes wrong
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(jsonString);
            }

            //Converting Json in Object form and giving it back if operation was successful.
            return Ok(createWeatherForecastObjectFromJson(jsonString)); 
        }


        /* Returns the medium weather of a place in a determined month.
         * GET call as .../WeatherAPI/GetMediumMonthWeather.
         * placeName: Defines a place. Could be the actual name of the place or latitude and longitude (e.g: 48.8567,2.3508).
         * date: Defines a date. (yyyy-mm)
         */
        [HttpGet("GetMediumMonthWeather")]
        public async Task<IActionResult> GetMediumMonthWeather(string placeName, string date)
        {
            int year = int.Parse(date.Substring(0, 4));
            int month = int.Parse(date.Substring(5, 2));
            int days = DateTime.DaysInMonth(year, month);

            string apiUrl;
            string jsonResponse;
            WeatherForecast[] weatherForecasts = new WeatherForecast[days];
            for(int i = 0; i < days; i++)
            {

                //Writing request
                apiUrl = $"http://localhost:5008/WeatherApi/GetWeather?placeName={placeName}&date={addDayToDate(date, i+1)}&hour={12}";

                //Awaiting response from WeatherAPI
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                //Collecting data into jsonResponse
                jsonResponse = await response.Content.ReadAsStringAsync();

                //Creating new WeatherForecast obj based on data
                weatherForecasts[i] = createWeatherForecastObjectFromJson(jsonResponse);

            }

            return Ok(weatherForecasts.Length);


        }




        //----------PRIVATE UTILITY METHODS-------------

        /*
         * Takes jsonString as input. Gives back WeatherForecast object.
         */
        private WeatherForecast createWeatherForecastObjectFromJson(string jsonString)
        {
            WeatherForecast wf = new WeatherForecast();

            using (var jsonDoc = JsonDocument.Parse(jsonString))
            {
                // Assume the JSON structure is like { "id": 123, "name": "John Doe", ... }
                JsonElement root = jsonDoc.RootElement;

                // Access specific fields
                wf.Country = root.GetProperty("location").GetProperty("country").GetString();
                wf.City = root.GetProperty("location").GetProperty("name").GetString();
                wf.TemperatureC = root.GetProperty("forecast").GetProperty("forecastday")[0].GetProperty("hour")[0].GetProperty("temp_c").GetDouble();
                wf.Date = root.GetProperty("forecast").GetProperty("forecastday")[0].GetProperty("hour")[0].GetProperty("time").GetString();
                wf.Weather = root.GetProperty("forecast").GetProperty("forecastday")[0].GetProperty("hour")[0].GetProperty("condition").GetProperty("text").GetString();

            }

            return wf;
        }

        /*
         * Takes a date in yyyy-mm format and a day as input. Returns yyyy-mm-dd string.
         */
        private string addDayToDate(string date, int day)
        {
            if(day < 10)
            {
                return date + "-0" + day;
            }
            return date + "-" + day;
        }
    }
}