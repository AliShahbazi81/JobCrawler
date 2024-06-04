using JobCrawler.Services.TelegramAPI.Config;
using JobCrawler.Services.TelegramAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace JobCrawler.Services.TelegramAPI.Services.Commands;

public class DonationCommand : IBotCommand
{
    public string Command => "/donation";
    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
    {
        var inlineKeyboards = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithUrl("\ud83d\udcb3 Stripe", "https://buy.stripe.com/6oE5kI6QMfYTbK0144"),
            }
        });
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "\ud83c\udf1f Donations are not required or mandatory, but are appreciated. If you want to support us, you can use the button below to show your generosity.\n",
            replyMarkup: inlineKeyboards,
            parseMode: ParseMode.Html
        );
    }
}