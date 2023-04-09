public record UserWithWeatherDto
{
    public int UserId { get; init; }
    public string Name { get; init; }
    public int CityId { get; init; }
    public string CityName { get; init; }
    public WeatherDto Weather { get; init; }
}