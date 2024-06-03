using JobCrawler.Services.TelegramAPI.Config;
using JobCrawler.Services.TelegramAPI.Services.Commands;
using JobCrawler.Services.TelegramAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace JobCrawler.Services.TelegramAPI.Services.Handler;

public class CommandHandlerService
{
    private readonly ITelegramBotClient _botClient;
    private readonly List<IBotCommand> _commands;

    public CommandHandlerService(IOptions<TelegramConfigs> options)
    {
        _botClient = new TelegramBotClient(options.Value.ApiToken);
        _commands = new List<IBotCommand>
        {
            new ContactUsCommand(),
            new ChannelsCommand(),
            new GroupCommand()
        };
    }
    
    public async Task HandleUpdateAsync(Update update)
    {
        Console.WriteLine($"Received the command {update.Message}");
        if (update.Type == UpdateType.Message && update.Message.Type == MessageType.Text)
        {
            var message = update.Message;
            if (message.Text != null && message.Text.StartsWith("/"))
            {
                await HandleCommandAsync(message);
            }
        }
    }

    private async Task HandleCommandAsync(Message message)
    {
        var command = _commands.FirstOrDefault(c => message.Text.StartsWith(c.Command, StringComparison.OrdinalIgnoreCase));
        if (command != null)
        {
            await command.ExecuteAsync(_botClient, message);
        }
        else
        {
            await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Sorry, I didn't understand that command."
            );
        }
    }
}