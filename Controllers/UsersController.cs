using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using TelegramBot_Api.Services;
using TelegramBot_Api.Models;

namespace TelegramBot_Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    private readonly WeatherService _weatherService;
    private readonly TelegramBotService _telegramBotService;
    private readonly WeatherNotificationService _weatherNotificationService;

    public UsersController(
        UserService userService, 
        WeatherService weatherService, 
        TelegramBotService telegramBotService, 
        WeatherNotificationService weatherNotificationService)
    {
        _userService = userService;
        _weatherService = weatherService;
        _telegramBotService = telegramBotService;
        _weatherNotificationService = weatherNotificationService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserWithHistory(long userId)
    {
        var user = await _userService.GetUserWithHistory(userId);
        if (user == null)
            return NotFound("User not found");

        return Ok(user);
    }

    [HttpPost("sendWeatherToUsers")]
    public async Task<IActionResult> SendWeatherToUsers([FromQuery] long? userId = null, [FromQuery] string city = "")
    {
        return await _weatherNotificationService.SendWeatherUpdatesAsync(userId, city);
    }
}
