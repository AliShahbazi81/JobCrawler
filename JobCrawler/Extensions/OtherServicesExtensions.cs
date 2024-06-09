using JobCrawler.Infrastructure.Crawler.Services;
using JobCrawler.Services.Crawler.Services;
using JobCrawler.Services.Crawler.Services.Interfaces;
using JobCrawler.Services.TelegramAPI.Config;
using JobCrawler.Services.TelegramAPI.Services;
using JobCrawler.Services.TelegramAPI.Services.Handler;
using JobCrawler.Services.TelegramAPI.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace JobScrawler.Extensions;

public static class OtherServicesExtensions
{
    public static IServiceCollection AddOtherServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        //! Services
        services.AddScoped<ICrawlerService, CrawlerService>();
        
        //! Telegram
            //! Crawler
        var telegramConfig = config.GetSection("Telegram");
        services.Configure<TelegramConfigs>(telegramConfig);
        services.AddScoped<ITelegramService, TelegramService>();
        
        var stripeConfig = config.GetSection("StripeInfo");
        services.Configure<StripeConfig>(stripeConfig);
        
            //! BOT
            services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token: telegramConfig["ApiToken"] ?? throw new InvalidOperationException()));
            services.AddSingleton<CommandHandlerService>();
            services.AddHostedService<BotService>();
        
        //! Managers
        services.AddSingleton<IHostedService, CrawlerManager>();
        
        return services;
    }
}