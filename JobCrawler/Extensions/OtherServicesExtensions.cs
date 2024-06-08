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
        /*var botClient = new TelegramBotClient("YOUR_API_TOKEN");
        var handlerService = new CommandHandlerService(botClient);

        botClient.StartReceiving(
            new DefaultUpdateHandler(handlerService.HandleUpdateAsync, HandleErrorAsync)
        );

        Console.WriteLine("Bot is up and running.");

        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                Telegram.Bot.Exceptions.ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }*/
        
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