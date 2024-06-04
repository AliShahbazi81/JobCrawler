using JobCrawler.Services.TelegramAPI.Services.Commands;
using JobCrawler.Services.TelegramAPI.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace JobCrawler.Services.TelegramAPI.Services.Handler;

public class CommandHandlerService
{
    private readonly ITelegramBotClient _botClient;
    private readonly List<IBotCommand> _commands;

    public CommandHandlerService(ITelegramBotClient botClient)
    {
        _botClient = botClient;
        _commands = new List<IBotCommand>
        {
            new StartCommand(),
            new ContactUsCommand(),
            new ChannelsCommand(),
            new GroupCommand()
        };
    }

    public async Task HandleUpdateAsync(Update update)
    {
        if (update.Type == UpdateType.Message && update.Message.Type == MessageType.Text)
        {
            var message = update.Message;
            if (message.Text.StartsWith("/"))
            {
                await HandleCommandAsync(message);
            }
        }
    }

    private async Task HandleCommandAsync(Message message)
    {
        var commandText = message.Text.Trim().ToLower();
        var command = _commands.FirstOrDefault(c => commandText.StartsWith(c.Command.ToLower()));
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