namespace WeatherReport.Models
{
    public class AvgWeather
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public double AvgTemperatureC { get; set; }

        public double AvgTemperatureF => 32 + (int)(AvgTemperatureC / 0.5556);

        public string AvgCondition { get; set; }
    }
}
