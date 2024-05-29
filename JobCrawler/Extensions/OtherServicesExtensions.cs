using JobCrawler.Services.Crawler.Services;
using JobCrawler.Services.Crawler.Services.Interfaces;
using JobCrawler.Services.TelegramAPI.Config;
using JobCrawler.Services.TelegramAPI.Services;
using JobCrawler.Services.TelegramAPI.Services.Interfaces;

namespace JobScrawler.Extensions;

public static class OtherServicesExtensions
{
    public static IServiceCollection AddOtherServices(this IServiceCollection services, IConfiguration config)
    {
        //! Services
        services.AddScoped<ICrawlerService, CrawlerService>();
        services.AddScoped<ITelegramService, TelegramService>();
        
        var telegramConfig = config.GetSection("Telegram");
        services.Configure<TelegramConfigs>(telegramConfig);
        
        return services;
    }
}