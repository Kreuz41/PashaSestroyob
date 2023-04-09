using System.ComponentModel.DataAnnotations;

public record CreateCityDto
{
    [Required]
    public string Name { get; init; }

    [Required]
    public double Latitude { get; init; }

    [Required]
    public double Longitude { get; init; }
}