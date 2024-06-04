using JobCrawler.Domain.Variables;
using JobCrawler.Services.TelegramAPI.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace JobCrawler.Services.TelegramAPI.Services.Commands;

public class ContactUsCommand : BotCommandBase
{
    public override string Command => "/contact_us";
    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
    {
        var inlineKeyboards = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithUrl("\u2705 LinkedIn", "https://www.linkedin.com/in/alishahbazi81/"),
                InlineKeyboardButton.WithUrl("\u2705 Telegram", "https://t.me/AliShahbazi81"),
            }
        });
        
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "You can contact us through the following channels: ",
            replyMarkup: inlineKeyboards,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html
        );
    }
}