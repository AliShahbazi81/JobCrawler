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
                InlineKeyboardButton.WithUrl(TelegramVarInfo.SoftwareDevelopmentChannelName, TelegramVarInfo.SoftwareDevelopmentChannelId),
            }
        });
        
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Here you can find the channels which are crawling the jobs:\n\n",
            replyMarkup: inlineKeyboards,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html
        );
    }
}