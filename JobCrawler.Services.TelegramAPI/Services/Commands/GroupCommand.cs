using JobCrawler.Domain.Variables;
using JobCrawler.Services.TelegramAPI.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace JobCrawler.Services.TelegramAPI.Services.Commands;

public class GroupCommand : BotCommandBase
{
    public override string Command => "/discussion";
    public override async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
    {
        var inlineKeyboards = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithUrl("\ud83d\udde3 General Discussion", "https://t.me/JCrawlerGroup"),
            }
        });

        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "By pressing the 'General Discussion' button, you will be redirected to our group. In the group we generally talk and share our idea about:\n\n" +
                  "\u2705 Resume and how to build them from scratch\n" +
                  "\u2705 Questions and their best answers for professional interviews\n" +
                  "\u2705 Our most recent experience from interviews meetings\n" +
                  "\u2705 MUST and MUST NOT about resume, interviews, and answers\n" +
                  "\u2705 General discussion all around these topics which may end up us getting hired!\n\n" +
                  "\ud83c\udf1f For preventing fake accounts and bots from joining, and keeping our channel most efficient, everyone has to be accepted in order to send/read messages. Your account may take up to 60 minutes to be accepted.\n" +
                  "Thank you for your understanding",
            replyMarkup: inlineKeyboards,
            parseMode: ParseMode.Html
        );
    }
}