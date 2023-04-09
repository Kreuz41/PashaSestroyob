using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PashaSestroyob.Data;
using PashaSestroyob.Data.Models;
using PashaSestroyob.Data.Models.Db;
using PashaSestroyob.Data.Models.Db.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PashaGandonEbaniyContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,
        };
    });

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

const string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoicXdlIiwiZXhwIjoxNjgxMDQwODM5LCJpc3MiOiJNeUF1dGhTZXJ2ZXIiLCJhdWQiOiJNeUF1dGhDbGllbnQifQ.p0omRMBIqdmQl1DdhBH7oRYVR4pg7fJerqYhgNWexLI";
app.UseMiddleware<BearerTokenMiddleware>(token);


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/users", async (PashaGandonEbaniyContext context, CreateUserDto dto) =>
{
    if (string.IsNullOrEmpty(dto.Name) || dto.Name.Length < 3)
    {
        return Results.BadRequest("Name should be at least 3 characters long");
    }

    var user = new User { Name = dto.Name, CityId = dto.CityId };
    context.Users.Add(user);
    await context.SaveChangesAsync();
    return Results.Created($"/users/{user.UserId}", user);
}).RequireAuthorization();


app.MapPost("/cities", [Authorize] async (PashaGandonEbaniyContext context, CreateCityDto dto) =>
{
    var validationResults = new List<ValidationResult>();
    if (!Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults))
    {
        return Results.BadRequest(validationResults.Select(v => v.ErrorMessage));
    }

    var city = new City { Name = dto.Name, Latitude = dto.Latitude, Longitude = dto.Longitude };
    context.Cities.Add(city);
    await context.SaveChangesAsync();
    return Results.Created($"/cities/{city.CityId}", city);
});

app.MapGet("/users/{id}", [Authorize] async (int id, PashaGandonEbaniyContext  context) =>
{
    var user = await context.Users.Include(u => u.City).FirstOrDefaultAsync(u => u.UserId == id);
    if (user == null)
    {
        return Results.NotFound();
    }

    var weather = await GetWeather(user.City.Latitude, user.City.Longitude);
    var result = new UserWithWeatherDto
    {
        UserId = user.UserId,
        Name = user.Name,
        CityId = user.CityId,
        CityName = user.City.Name,
        Weather = weather
    };
    return Results.Ok(result);
});

app.MapGet("/users", [Authorize] async (double lat, double lon, double r, PashaGandonEbaniyContext context) =>
{

    var usersWithinRadius = context.Users
        .Join(
            inner: context.Cities,
            outerKeySelector: u => u.CityId,
            innerKeySelector: c => c.CityId,
            resultSelector: (u, c) => new { User = u, City = c }).AsEnumerable()
        .Where(u => Distance.GetDistance(lat, lon, u.City.Latitude, u.City.Longitude) <= r)
        .Select(u => u.User);

    var result = usersWithinRadius.Select(u => new UserDto
    {
        UserId = u.UserId,
        Name = u.Name,
        CityId = u.CityId,
        CityName = u.City.Name
    });

    return Results.Ok(result);
});


app.Run();

async Task<WeatherDto> GetWeather(double latitude, double longitude)
{
    string apiKey = "139af0c471780d8790271dda24a26e4a";
    string url = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}";

    using (var client = new HttpClient())
    {
        var response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            var weatherData = JsonSerializer.Deserialize<OpenWeatherMapResponse>(responseContent);

            var weather = new WeatherDto
            {
                Description = weatherData.weather.FirstOrDefault().description,
                Temperature = (int)weatherData.main.temp,
                Humidity = weatherData.main.humidity,
                WindSpeed = weatherData.wind.speed
            };

            return weather;
        }
    }

    return null;
}