namespace Verd.Api.DTOs.Weather;

public record WeatherDto(
    int Temp,
    string Condition,
    int Humidity,
    int UvIndex,
    string UvLabel,
    int SoilMoisture,
    int WindSpeed,
    int Aqi,
    int FeelsLike,
    IEnumerable<ForecastDayDto> Forecast
);

public record ForecastDayDto(
    string Day,
    string Date,
    string Icon,
    int TempHi,
    int TempLo,
    bool Active
);
