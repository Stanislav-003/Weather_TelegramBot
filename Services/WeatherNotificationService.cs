using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using TelegramBot_Api.Models;

namespace TelegramBot_Api.Services;

public class WeatherNotificationService
{
    private readonly UserService _userService;
    private readonly WeatherService _weatherService;
    private readonly TelegramBotService _telegramBotService;

    public WeatherNotificationService(UserService userService, WeatherService weatherService, TelegramBotService telegramBotService)
    {
        _userService = userService;
        _weatherService = weatherService;
        _telegramBotService = telegramBotService;
    }

    public async Task<IActionResult> SendWeatherUpdatesAsync(long? userId, string city)
    {
        if (string.IsNullOrEmpty(city))
            return new BadRequestObjectResult("City is required");

        var userOrUsers = new List<TelegramBot_Api.Models.User>();

        if (userId != null)
        {
            var user = await _userService.GetUserById(userId.Value);
            if (user != null)
            {
                userOrUsers.Add(user);
            }
        }
        else
        {
            userOrUsers = await _userService.GetAllUsers();
        }

        if (userOrUsers?.Count == 0)
            return new NotFoundObjectResult("Users not found");

        foreach (var user in userOrUsers!)
        {
            var weatherInfo = await _weatherService.GetWeatherAsync(user.Id, city, DateTime.UtcNow);
            await _telegramBotService.SendWeatherUpdateAsync(user.Id, weatherInfo);
        }

        return new OkObjectResult("Weather update sent");
    }
}