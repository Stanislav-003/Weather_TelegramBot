using Telegram.Bot;

namespace TelegramBot_Api.Commands;

public class MenuCommand : IBotCommand
{
    public async Task ExecuteAsync(
        ITelegramBotClient botClient,
        long chatId,
        string[] args,
        CancellationToken cancellationToken)
    {
        string menuMessage = "📜 Доступні команди:\n" +
                             "🔹 /start - Інформація про бота\n" +
                             "🔹 /menu - Показати список команд\n" +
                             "🔹 /weather - Отримати прогноз погоди";

        await botClient.SendMessage(
            chatId,
            menuMessage,
            cancellationToken: cancellationToken);
    }
}