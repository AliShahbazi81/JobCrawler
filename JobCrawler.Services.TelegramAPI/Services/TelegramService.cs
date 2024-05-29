using JobCrawler.Services.TelegramAPI.Config;
using JobCrawler.Services.TelegramAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace JobCrawler.Services.TelegramAPI.Services;

public class TelegramService : ITelegramService
{
    private readonly ITelegramBotClient _botClient;
    private readonly string _softwareDevelopmentChannelId;

    public TelegramService(
        ITelegramBotClient botClient, 
        IOptions<TelegramConfigs> options)
    {
        _botClient = botClient;
        _softwareDevelopmentChannelId = options.Value.SoftwareDevelopmentChannelId;
    }

    public async Task<int> SendAsync(string message)
    {
        var messageResponse = await _botClient.SendTextMessageAsync(
            _softwareDevelopmentChannelId,
            message,
            parseMode: ParseMode.Markdown);

        return messageResponse.MessageId;
    }

    public async Task<bool> UpdateMessageAsync(int messageId, string message)
    {
        try
        {
            await _botClient.EditMessageTextAsync(
                _softwareDevelopmentChannelId,
                messageId,
                message,
                ParseMode.Markdown,
                replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Refresh", "refresh_" + DateTime.UtcNow.Ticks))
            );

            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}