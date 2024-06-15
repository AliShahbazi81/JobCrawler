using System.Diagnostics.Metrics;
using JobCrawler.Services.TelegramAPI.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace JobCrawler.Services.TelegramAPI.Services.Commands;

public class StartCommand : IBotCommand
{
    public string Command => "/start";
    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
    {
        var inlineKeyboards = new InlineKeyboardMarkup(new[]
        {
            [
                InlineKeyboardButton.WithCallbackData("\ud83d\udd0e Channels", "/channels"),
                InlineKeyboardButton.WithCallbackData("\ud83d\udde3 Discussion", "/discussion")
            ],
            new[]
            {
                InlineKeyboardButton.WithCallbackData("\ud83d\udcb3 Donation", "/donation"),
                InlineKeyboardButton.WithCallbackData("\ud83d\udcbb Contact Us", "/contact_us"),
            }
        });
        
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Welcome !\n" +
                  "Thank you for choosing us. Just for a quick heads up, this bot is in alpha version (v0.1.0) and we are continuously working on it to make it better.\n",
            replyMarkup: inlineKeyboards,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html
        );
        
        // Get clientId from Telegram message
        var userId = message.From.Id;
    }

    private async Task RegisterUserAsync(long userId)
    {
        // Take userId from the message and save it to the database
        var user = new User
        {
            Id = userId
        };
    }
}