using JobCrawler.Infrastructure.Crawler.Services;
using JobCrawler.Infrastructure.Crawler.Services.Interfaces;
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
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        //! Services
        services.AddScoped<ICrawlerService, CrawlerService>();
        
        var telegramConfig = config.GetSection("Telegram");
        services.Configure<TelegramConfigs>(telegramConfig);
        services.AddScoped<ITelegramService, TelegramService>();
        
        //! Managers
        services.AddSingleton<IHostedService, CrawlerManager>();
        
        return services;
    }
}