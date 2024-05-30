using JobCrawler.Services.Crawler.DTO;
using JobCrawler.Services.TelegramAPI.Config;
using JobCrawler.Services.TelegramAPI.Services.Interfaces;
using JobCrawler.Services.TelegramAPI.Templates;
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
        IOptions<TelegramConfigs> options)
    {
        _botClient = new TelegramBotClient(options.Value.ApiToken);
        _softwareDevelopmentChannelId = options.Value.SoftwareDevelopmentChannelId;
    }

    public async Task SendJobPostsAsync(List<JobDto> jobs)
    {
        foreach (var job in jobs)
        {
            var message = JobBoardingTemplate.CreateJobMessage(job);
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithUrl("View Job", job.Url)
            });

            await _botClient.SendTextMessageAsync(
                chatId: _softwareDevelopmentChannelId,
                text: message,
                replyMarkup: inlineKeyboard,
                parseMode: ParseMode.Html
            );
        }
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