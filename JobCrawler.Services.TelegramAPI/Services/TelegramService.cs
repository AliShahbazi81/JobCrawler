using JobCrawler.Services.Crawler.DTO;
using JobCrawler.Services.TelegramAPI.Config;
using JobCrawler.Services.TelegramAPI.Services.Interfaces;
using JobCrawler.Services.TelegramAPI.Templates;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
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

    public async Task SendJobPostsAsync(JobDto job)
    {
            /*var message = JobBoardingTemplate.CreateJobMessage(job);
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithUrl("View Job", job.Url)
            });

            var success = false;
            var retryCount = 0;
            const int maxRetryAttempts = 5;

            while (!success && retryCount < maxRetryAttempts)
            {
                try
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: _softwareDevelopmentChannelId,
                        text: message,
                        replyMarkup: inlineKeyboard,
                        parseMode: ParseMode.Html,
                        protectContent: true
                    );
                    success = true;
                }
                catch (ApiRequestException ex) when (ex.Message.Contains("Too Many Requests"))
                {
                    retryCount++;
                    var retryAfter = ex.Parameters?.RetryAfter ?? 5; 
                    await Task.Delay(retryAfter * 1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    break; 
                }
            }*/
    }
}