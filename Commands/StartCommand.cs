using Telegram.Bot;

namespace TelegramBot_Api.Commands;

public class StartCommand : IBotCommand
{
    public async Task ExecuteAsync(
        ITelegramBotClient botClient,
        long chatId,
        string[] args,
        CancellationToken cancellationToken)
    {
        string welcomeMessage = "👋 Вітаю! Я бот для отримання інформації про погоду. \n\n" +
                                "📌 Використовуйте /menu, щоб побачити список доступних команд.";

        await botClient.SendMessage(
            chatId,
            welcomeMessage,
            cancellationToken: cancellationToken);
    }
}
