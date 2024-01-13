namespace WeatherReport.Models
{
    public class WeatherForecast
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Date { get; set; }

        public double TemperatureC { get; set; }

        public double TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Weather { get; set; }
    }
}