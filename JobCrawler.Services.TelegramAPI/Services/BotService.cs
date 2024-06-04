using JobCrawler.Services.TelegramAPI.Config;
using JobCrawler.Services.TelegramAPI.Services.Handler;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace JobCrawler.Services.TelegramAPI.Services;

public class BotService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly CommandHandlerService _commandHandler;

    public BotService(CommandHandlerService commandHandler, IOptions<TelegramConfigs> options)
    {
        _botClient = new TelegramBotClient(options.Value.ApiToken);
        _commandHandler = commandHandler;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _botClient.StartReceiving(
            async (_, update, _) => await HandleUpdateAsync(update),
            async (_, exception, _) => await HandleErrorAsync(exception),
            new ReceiverOptions(),
            stoppingToken
        );

        Console.WriteLine("Bot is up and running.");
    }

    private async Task HandleUpdateAsync(Update update)
    {
        await _commandHandler.HandleUpdateAsync(update);
    }

    private Task HandleErrorAsync(Exception exception)
    {
        Console.WriteLine($"An error occurred: {exception.Message}");
        return Task.CompletedTask;
    }
}