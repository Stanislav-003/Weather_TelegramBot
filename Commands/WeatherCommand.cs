using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramBot_Api.Models;
namespace TelegramBot_Api.Commands;

public class WeatherCommand(CommandHandler commandHandler) : IBotCommand
{
    public async Task ExecuteAsync(ITelegramBotClient botClient, long chatId, string[] args, CancellationToken cancellationToken)
    {
        commandHandler.SetUserState(
            chatId,
            UserState.AwaitingCity);

        await botClient.SendMessage(
            chatId,
            "🌍 Введіть назву міста, щоб отримати інформацію про погоду:",
            cancellationToken: cancellationToken);
    }
}
