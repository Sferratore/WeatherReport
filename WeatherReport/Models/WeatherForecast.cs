namespace WeatherReport.Models
{
    public class WeatherForecast
    {
        public string Country { get; set; }
        public string City { get; set; }
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Weather { get; set; }
    }
}