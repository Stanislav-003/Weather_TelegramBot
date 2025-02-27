using Telegram.Bot.Polling;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot_Api.Commands;

namespace TelegramBot_Api.Handlers;

public class UpdateHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly CommandHandler _commandHandler;

    public UpdateHandler(
        ITelegramBotClient botClient,
        CommandHandler commandHandler)
    {
        _botClient = botClient;
        _commandHandler = commandHandler;
    }

    public async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken)
    {
        if (update.Message is { } message && message.Text is { } text)
        {
            var date = update.Message.Date;

            var chatId = message.Chat.Id;
            await _commandHandler.HandleCommandAsync(
                botClient,
                date,
                chatId,
                text,
                cancellationToken);
        }
    }

    public Task HandleErrorAsync(
        ITelegramBotClient botClient,
        Exception exception,
        HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Ошибка: {exception.Message}");
        return Task.CompletedTask;
    }
}