using JobCrawler.Services.TelegramAPI.Config;

namespace JobScrawler.Extensions;

public static class OtherServicesExtensions
{
    public static IServiceCollection AddOtherServices(this IServiceCollection services, IConfiguration config)
    {
        var telegramConfig = config.GetSection("Telegram");
        services.Configure<TelegramConfigs>(telegramConfig);
        
        return services;
    }
}