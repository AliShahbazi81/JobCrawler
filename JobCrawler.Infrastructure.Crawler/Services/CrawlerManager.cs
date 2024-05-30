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
}