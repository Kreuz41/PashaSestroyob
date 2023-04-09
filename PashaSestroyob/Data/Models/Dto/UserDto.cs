public record UserDto
{
    public int UserId { get; init; }
    public string Name { get; init; }
    public int CityId { get; init; }
    public string CityName { get; init; }
}