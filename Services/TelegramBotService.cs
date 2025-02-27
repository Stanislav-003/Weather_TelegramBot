using Telegram.Bot;
using TelegramBot_Api.Models;

namespace TelegramBot_Api.Services;

public class TelegramBotService
{
    private readonly ITelegramBotClient _botClient;

    public TelegramBotService(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task SendWeatherUpdateAsync(long userId, string weatherInfo)
    {        
        await _botClient.SendMessage(userId, weatherInfo);
    }
}
