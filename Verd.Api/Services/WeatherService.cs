using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Verd.Api.DTOs.Weather;

namespace Verd.Api.Services;

public class WeatherService(IHttpClientFactory factory)
{
    private static readonly string[] Days = ["SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT"];
    private static readonly string[] Months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public async Task<WeatherDto?> GetForLocationAsync(string location)
    {
        var http = factory.CreateClient();

        // 1. Geocode
        var geoUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(location)}&count=1&language=en&format=json";
        var geoJson = await http.GetStringAsync(geoUrl);
        var geo = JsonSerializer.Deserialize<GeoResponse>(geoJson, JsonOpts);
        var place = geo?.Results?.FirstOrDefault();
        if (place is null) return null;

        // 2. Fetch weather. Format coords with invariant culture so the decimal
        // separator is always "." — a comma (in some locales) produces an invalid
        // URL and a 400 from open-meteo.
        var lat = place.Latitude.ToString(CultureInfo.InvariantCulture);
        var lon = place.Longitude.ToString(CultureInfo.InvariantCulture);
        var url = $"https://api.open-meteo.com/v1/forecast" +
            $"?latitude={lat}&longitude={lon}" +
            "&daily=weathercode,temperature_2m_max,temperature_2m_min" +
            "&hourly=temperature_2m,relativehumidity_2m,windspeed_10m,uv_index,apparent_temperature,soil_moisture_0_to_1cm" +
            // Pinned rather than relying on the API default: the whole app reports
            // Celsius, and the conversion belongs here in the data layer.
            "&temperature_unit=celsius" +
            "&timezone=auto&forecast_days=7";

        var weatherJson = await http.GetStringAsync(url);
        var w = JsonSerializer.Deserialize<OpenMeteoResponse>(weatherJson, JsonOpts);
        if (w is null) return null;

        var hourIndex = DateTime.Now.Hour;
        var hourly = w.Hourly;
        var temp = (int)Math.Round(hourly.Temperature2m[hourIndex]);
        var feelsLike = (int)Math.Round(hourly.ApparentTemperature[hourIndex]);
        var humidity = (int)Math.Round(hourly.Relativehumidity2m[hourIndex]);
        var windSpeed = (int)Math.Round(hourly.Windspeed10m[hourIndex]);
        var uvIndex = (int)Math.Round(hourly.UvIndex[hourIndex]);
        var soilMoisture = (int)Math.Min(100, Math.Round(hourly.SoilMoisture0To1Cm[hourIndex] * 100));

        var daily = w.Daily;
        var forecast = daily.Time.Select((dateStr, i) =>
        {
            var d = DateTime.Parse(dateStr);
            var isToday = i == 0;
            return new ForecastDayDto(
                Day: isToday ? "TODAY" : Days[(int)d.DayOfWeek],
                Date: $"{Months[d.Month - 1]} {d.Day}",
                Icon: WmoIcon(daily.Weathercode[i]),
                TempHi: (int)Math.Round(daily.Temperature2mMax[i]),
                TempLo: (int)Math.Round(daily.Temperature2mMin[i]),
                Active: isToday
            );
        });

        var uvLabel = uvIndex >= 8 ? "Very High" : uvIndex >= 6 ? "High" : uvIndex >= 3 ? "Moderate" : "Low";
        var condition = WmoCondition(daily.Weathercode[0]);

        return new WeatherDto(
            Temp: temp,
            Condition: condition,
            Humidity: humidity,
            UvIndex: uvIndex,
            UvLabel: uvLabel,
            SoilMoisture: soilMoisture,
            WindSpeed: windSpeed,
            Aqi: 0,
            FeelsLike: feelsLike,
            Forecast: forecast
        );
    }

    private static string WmoIcon(int code) => code switch
    {
        0 or 1 => "sun",
        2 => "cloud-sun",
        3 or 45 or 48 => "cloud",
        >= 51 and <= 67 => "rain",
        >= 71 and <= 77 => "cloud",
        >= 80 and <= 82 => "rain",
        85 or 86 => "cloud",
        >= 95 => "rain",
        _ => "cloud-sun",
    };

    private static string WmoCondition(int code) => code switch
    {
        0 => "Clear Sky",
        1 => "Mainly Clear",
        2 => "Partly Cloudy",
        3 => "Overcast",
        45 or 48 => "Foggy",
        >= 51 and <= 55 => "Drizzle",
        >= 61 and <= 67 => "Rainy",
        >= 71 and <= 77 => "Snowy",
        >= 80 and <= 82 => "Rain Showers",
        >= 95 => "Thunderstorm",
        _ => "Cloudy",
    };
}

// Open-Meteo response models
file record GeoResponse([property: JsonPropertyName("results")] List<GeoPlace>? Results);
file record GeoPlace(double Latitude, double Longitude, string Name);

file record OpenMeteoResponse(OpenMeteoHourly Hourly, OpenMeteoDaily Daily);

file record OpenMeteoHourly(
    [property: JsonPropertyName("temperature_2m")] double[] Temperature2m,
    [property: JsonPropertyName("apparent_temperature")] double[] ApparentTemperature,
    [property: JsonPropertyName("relativehumidity_2m")] double[] Relativehumidity2m,
    [property: JsonPropertyName("windspeed_10m")] double[] Windspeed10m,
    [property: JsonPropertyName("uv_index")] double[] UvIndex,
    [property: JsonPropertyName("soil_moisture_0_to_1cm")] double[] SoilMoisture0To1Cm
);

file record OpenMeteoDaily(
    string[] Time,
    int[] Weathercode,
    [property: JsonPropertyName("temperature_2m_max")] double[] Temperature2mMax,
    [property: JsonPropertyName("temperature_2m_min")] double[] Temperature2mMin
);
