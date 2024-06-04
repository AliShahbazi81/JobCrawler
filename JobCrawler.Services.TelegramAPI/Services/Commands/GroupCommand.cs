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
                InlineKeyboardButton.WithUrl("General Discussion", TelegramVarInfo.GeneralDiscussionChannelId),
            }
        });

        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "You can find our discussion group down below. Members will talk about resume, interview questions, useful resources, and their experience for professional jobs.\n\n",
            replyMarkup: inlineKeyboards,
            parseMode: ParseMode.Html
        );
    }
}