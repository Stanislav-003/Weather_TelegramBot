using Microsoft.Extensions.Options;
using System.Net.Http;
using Telegram.Bot.Types;
using TelegramBot_Api.Configuration;
using TelegramBot_Api.Repository;

namespace TelegramBot_Api.Services;

public class WeatherService(
    HttpClient httpClient, 
    IOptions<WeatherConfig> config,
    WeatherRepository weatherRepository)
{
    private readonly string _apiKey = config.Value.ApiKey;

    public async Task<string> GetWeatherAsync(long userId, string city, DateTime date)
    {
        var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric&lang=ru";
        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            return $"Помилка при запиті погоди для міста {city}: {response.StatusCode}";
        }

        var weatherResponse = await response.Content.ReadFromJsonAsync<WeatherResponse>();
        if (weatherResponse == null)
        {
            return "Не вдалось розібрати відповідь від сервісу погоди.";
        }

        var weatherInfo = $"🌍 Місто: {weatherResponse.Name}\n" +
                          $"🌡 Температура: {weatherResponse.Main.Temp}°C\n" +
                          $"☁️ Опис: {weatherResponse.Weather[0].Description}\n" +
                          $"💨 Вітер: {weatherResponse.Wind.Speed} м/с";

        await weatherRepository.AddWeatherRequestAsync(
            userId,
            city,
            weatherResponse.Main.Temp,
            weatherResponse.Weather[0].Description,
            weatherResponse.Wind.Speed,
            date);

        return weatherInfo;
    }
}

public class WeatherResponse
{
    public string Name { get; set; } = null!;
    public MainInfo Main { get; set; } = null!;
    public WeatherInfo[] Weather { get; set; } = null!;
    public WindInfo Wind { get; set; } = null!;
}

public class MainInfo { public float Temp { get; set; } }
public class WeatherInfo { public string Description { get; set; } = null!; }
public class WindInfo { public float Speed { get; set; } }
