namespace WeatherReport
{
    /*
     * WeatherApiSettings is a class used to store weather api settings stored in appsettings.json.
     */
public class WeatherApiSettings
    {
        public string WeatherAPIKey { get; set; }
        public string HistoryUrl { get; set; }
    }
}
