using Telegram.Bot;
using TelegramBot_Api.Services;
using TelegramBot_Api.Models;
using System.Collections.Concurrent;
namespace TelegramBot_Api.Commands;

public class CommandHandler
{
    private readonly Dictionary<string, IBotCommand> _commands;
    private readonly ConcurrentDictionary<long, UserState> _userStates = new();
    private readonly WeatherService _weatherService;

    public CommandHandler(WeatherService weatherService)
    {
        _weatherService = weatherService;
        _commands = new()
        {
            ["/weather"] = new WeatherCommand(this),
            ["/menu"] = new MenuCommand(),
            ["/start"] = new StartCommand()
        };
    }

    public async Task HandleCommandAsync(
        ITelegramBotClient botClient,
        DateTime date,
        long chatId,
        string text,
        CancellationToken cancellationToken)
    {
        if (_commands.TryGetValue(text, out var handler))
        {
            await handler.ExecuteAsync(botClient, chatId, Array.Empty<string>(), cancellationToken);
            return;
        }

        switch (_userStates.GetValueOrDefault(chatId, UserState.None))
        {
            case UserState.AwaitingCity:
                await HandleCityInputAsync(
                    botClient, 
                    date, 
                    chatId, 
                    text, 
                    cancellationToken);
                break;
            default:
                await botClient.SendMessage(
                    chatId, 
                    "❌ Невідома команда. Використовуйте /menu", 
                    cancellationToken: cancellationToken);
                await new MenuCommand().ExecuteAsync(
                    botClient, 
                    chatId, 
                    Array.Empty<string>(), 
                    cancellationToken);
                break;
        }
    }

    public void SetUserState(
        long chatId,
        UserState state)
    {
        _userStates[chatId] = state;
    }

    private async Task HandleCityInputAsync(
        ITelegramBotClient botClient,
        DateTime date,
        long chatId,
        string city,
        CancellationToken cancellationToken)
    {
        var weatherInfo = await _weatherService.GetWeatherAsync(chatId, city, date);

        await botClient.SendMessage(
            chatId,
            weatherInfo,
            cancellationToken: cancellationToken);

        _userStates.TryRemove(chatId, out _);

        // Виклик команди меню щоб дати можливість користувачеві обрати нову команду
        var menuCommand = new MenuCommand();
        await menuCommand.ExecuteAsync(
            botClient,
            chatId,
            Array.Empty<string>(),
            cancellationToken);
    }
}

