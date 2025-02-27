using Telegram.Bot;

namespace TelegramBot_Api.Commands;

public interface IBotCommand
{
    Task ExecuteAsync(ITelegramBotClient botClient, long chatId, string[] args, CancellationToken cancellationToken);
}
