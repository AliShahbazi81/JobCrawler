using JobCrawler.Services.TelegramAPI.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace JobCrawler.Services.TelegramAPI.Services.Commands;

public class StartCommand : IBotCommand
{
    public string Command => "/start";
    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
    {
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Welcome! Use the following commands to interact with the bot:\n" +
                  "/contact_us - Contact information\n" +
                  "/channels - List of available channels\n" +
                  "/discussion - Discussion channels",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html
        );
    }
}