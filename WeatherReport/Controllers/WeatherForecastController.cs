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
            return Ok(createWeatherForecastObjectFromJsonExternal(jsonString)); 
        }


        /* Returns the medium weather of a place in a determined month.
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

                if (response.IsSuccessStatusCode)
                {
                    //Creating new WeatherForecast obj based on data
                    weatherForecasts[i] = createWeatherForecastObjectFromJsonInternal(jsonResponse);
                }
                else
                {
                    return BadRequest(jsonResponse);
                }       

            }

            //Creating and returning results
            return Ok(createAvgWeather(weatherForecasts));

        }


        /* Returns the medium weather of a place in a determined year.
         * placeName: Defines a place. Could be the actual name of the place or latitude and longitude (e.g: 48.8567,2.3508).
         * date: Defines a date. (yyyy)
         */
        [HttpGet("GetMediumYearWeather")]
        public async Task<IActionResult> GetMediumYearWeather(string placeName, string date)
        {

            string apiUrl;
            string jsonResponse;
            WeatherForecast[] weatherForecasts = new WeatherForecast[12]; //This contains the averages of the months

            for (int i = 0; i < 12; i++)
            {

                //Writing request
                apiUrl = $"http://localhost:5008/WeatherApi/GetMediumMonthWeather?placeName={placeName}&date={date + (i < 10 ? "-0" + i : "-" + i)}";  //Using ternary operator to check if i is < 10. In case we will need that extra zero on string.

                //Awaiting response from WeatherAPI
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                //Collecting data into jsonResponse
                jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    //Creating new WeatherForecast obj based on data
                    weatherForecasts[i] = createWeatherForecastObjectFromJsonInternal(jsonResponse);
                }
                else
                {
                    return BadRequest(jsonResponse);
                }

            }

            //Creating and returning results
            return Ok(createAvgWeather(weatherForecasts));


        }




        //----------PRIVATE UTILITY METHODS-------------

        /*
         * Takes jsonString as input. Json must be the one from external WeatherAPI (NOT WeatherReport)
         * Gives back WeatherForecast object.
         */
        private WeatherForecast createWeatherForecastObjectFromJsonExternal(string jsonString)
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
         * Takes jsonString as input. Json must be the one from internal WeatherReport (NOT WeatherAPI)
         * Gives back WeatherForecast object.
         */
        private WeatherForecast createWeatherForecastObjectFromJsonInternal(string jsonString)
        {
            WeatherForecast wf = new WeatherForecast();

            using (var jsonDoc = JsonDocument.Parse(jsonString))
            {
                // Assume the JSON structure is like { "id": 123, "name": "John Doe", ... }
                JsonElement root = jsonDoc.RootElement;

                // Access specific fields
                wf.Country = root.GetProperty("country").GetString();
                wf.City = root.GetProperty("city").GetString();
                wf.TemperatureC = root.GetProperty("temperatureC").GetDouble();
                wf.Date = root.GetProperty("date").GetString();
                wf.Weather = root.GetProperty("weather").GetString();

            }

            return wf;
        }


        /*
         * Creates AvgWeather object from a WeatherForecast array.
         */
        private AvgWeather createAvgWeather(WeatherForecast[] array)
        {
            AvgWeather avgWf = new AvgWeather();

            avgWf.City = array[0].City;
            avgWf.Country = array[0].Country;
            avgWf.FromDate = array[0].Date;
            avgWf.ToDate = array[array.Length - 1].Date;

            //Calculating average temperature
            avgWf.AvgTemperatureC = 0;
            for (int i = 0; i < array.Length; i++)
            {
                avgWf.AvgTemperatureC += array[i].TemperatureC;
            }
            avgWf.AvgTemperatureC /= array.Length;

            //Calculating most common weather condition
            var weatherConditionCounter = new Dictionary<string, int>();
            foreach (WeatherForecast wthFc in array)
            {
                if (weatherConditionCounter.ContainsKey(wthFc.Weather)) //If the key, which is wthFc's Weather condition, is already present in the dictionary, we increment
                {
                    weatherConditionCounter[wthFc.Weather]++;
                }
                else  //Else, we create it and assign 1
                {
                    weatherConditionCounter[wthFc.Weather] = 1;
                }
            }
            // Using the Aggregate method to find the most frequent weather condition.
            // The Aggregate method iterates through each key-value pair in the weatherConditionCounter dictionary.
            // In each iteration, it compares the current pair (x) with the next pair (y).
            // The lambda expression (x, y) => x.Value > y.Value ? x : y decides which pair to carry forward to the next comparison.
            // If x's value (the count of occurrences) is greater than y's, x is carried forward; otherwise, y is.
            // This process repeats until the pair with the highest count is found.
            // Finally, .Key is used to extract the key (the weather condition string) from that pair.
            avgWf.AvgCondition = weatherConditionCounter.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

            //Give result
            return avgWf;
        }


        private AvgWeather createAvgWeatherFromJson(string jsonString)
        {
            AvgWeather awf = new AvgWeather();

            using (var jsonDoc = JsonDocument.Parse(jsonString))
            {
                // Assume the JSON structure is like { "id": 123, "name": "John Doe", ... }
                JsonElement root = jsonDoc.RootElement;

                // Access specific fields
                awf.Country = root.GetProperty("country").GetString();
                awf.City = root.GetProperty("city").GetString();
                awf.FromDate = root.GetProperty("fromDate").GetString();
                awf.ToDate = root.GetProperty("toDate").GetString();
                awf.AvgTemperatureC = root.GetProperty("avgTemperatureC").GetDouble();
                awf.AvgCondition = root.GetProperty("avgCondition").GetString();

            }

            return awf;
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