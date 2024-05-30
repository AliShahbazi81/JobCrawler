using JobCrawler.Domain.Results;
using JobCrawler.Infrastructure.Crawler.Services.Interfaces;
using JobCrawler.Services.Crawler.Services.Interfaces;
using JobCrawler.Services.TelegramAPI.Services.Interfaces;

namespace JobCrawler.Infrastructure.Crawler.Services;

public class CrawlerManager : ICrawlerManager
{
    private readonly ITelegramService _telegramService;
    private readonly ICrawlerService _crawlerService;

    public CrawlerManager(
        ICrawlerService crawlerService, 
        ITelegramService telegramService)
    {
        _crawlerService = crawlerService;
        _telegramService = telegramService;
    }
    
    public async Task<Result<bool>> CrawlAndSendJobPostsAsync()
    {
        var jobs = await _crawlerService.GetJobsAsync();
        await _telegramService.SendJobPostsAsync(jobs);
        return Result<bool>.Success(true);
    }
}