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
            new GroupCommand(),
            new DonationCommand(),
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
        else if (update.Type == UpdateType.CallbackQuery)
        {
            var callbackQuery = update.CallbackQuery;
            if (callbackQuery != null)
            {
                await HandleCallbackQueryAsync(callbackQuery);
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

    private async Task HandleCallbackQueryAsync(CallbackQuery callbackQuery)
    {
        var command = callbackQuery.Data;
        if (!string.IsNullOrEmpty(command))
        {
            var message = new Message
            {
                Chat = callbackQuery.Message.Chat,
                MessageId = callbackQuery.Message.MessageId,
                Text = command
            };
            await HandleCommandAsync(message);
        }
    }
}