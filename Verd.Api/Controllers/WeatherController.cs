using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Verd.Api.DTOs.Weather;

namespace Verd.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WeatherController : ControllerBase
{
    // Returns mock weather data — swap GetCurrent() body with a real
    // weather API call (e.g. OpenWeatherMap) when ready.
    [HttpGet]
    public ActionResult<WeatherDto> GetCurrent()
    {
        var weather = new WeatherDto(
            Temp: 72,
            Condition: "Partly Cloudy",
            Humidity: 45,
            UvIndex: 6,
            UvLabel: "High",
            SoilMoisture: 32,
            WindSpeed: 8,
            Aqi: 24,
            FeelsLike: 75,
            Forecast:
            [
                new("TODAY", "May 12", "sun",      72, 54, true),
                new("MON",   "May 13", "cloud-sun", 68, 51, false),
                new("TUE",   "May 14", "rain",      62, 49, false),
                new("WED",   "May 15", "cloud",     65, 50, false),
                new("THU",   "May 16", "sun",       70, 52, false),
            ]
        );

        return Ok(weather);
    }
}
