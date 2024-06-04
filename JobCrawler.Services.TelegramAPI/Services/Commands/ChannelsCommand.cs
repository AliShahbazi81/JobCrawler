using JobCrawler.Domain.Variables;
using JobCrawler.Services.TelegramAPI.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace JobCrawler.Services.TelegramAPI.Services.Commands;

public class ChannelsCommand : BotCommandBase
{
    public override string Command => "/channels";
    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
    {
        var inlineKeyboards = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithUrl("\ud83d\udcbb Software Development", "https://t.me/JCrawler_Computer"),
            }
        });
        
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "\u2705 Here you can find the channels which are crawling the jobs:\n\n" +
                  "Be aware that each channel crawls related jobs every 30 minutes. If you cannot find your field, do not worry, we are continuously working on expanding the bot to cover almost all of the fields.\n" +
                  "Also, you can ask us to support your field as soon as possible! For doing so, you can send us a message:\n\n" +
                  "@AliShahbazi81",
            replyMarkup: inlineKeyboards,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html
        );
    }
}