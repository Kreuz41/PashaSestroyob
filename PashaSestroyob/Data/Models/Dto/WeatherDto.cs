public class WeatherDto
{
    public string Description { get; set; }
    public double Temperature { get; set; }
    public double FeelsLikeTemperature { get; set; }
    public double Pressure { get; set; }
    public double Humidity { get; set; }
    public double WindSpeed { get; set; }
    public string WindDirection { get; set; }
}

public class OpenWeatherMapResponse
{
    public IEnumerable<OpenWeatherMapWeather> weather { get; set; }
    public OpenWeatherMapMain main { get; set; }
    public OpenWeatherMapWind wind { get; set; }
    public string name { get; set; }
}

public class OpenWeatherMapWeather
{
    public string main { get; set; }
    public string description { get; set; }
}

public class OpenWeatherMapMain
{
    public double temp { get; set; }
    public double feelsLike { get; set; }
    public double pressure { get; set; }
    public double humidity { get; set; }
}

public class OpenWeatherMapWind
{
    public double speed { get; set; }
    public double deg { get; set; }
}