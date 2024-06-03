using Telegram.Bot;
using Telegram.Bot.Types;

namespace JobCrawler.Services.TelegramAPI.Services.Interfaces;

public interface IBotCommand
{
    string Command { get; }
    Task ExecuteAsync(ITelegramBotClient botClient, Message message);
}

public abstract class BotCommandBase : IBotCommand
{
    public abstract string Command { get; }

    public abstract Task ExecuteAsync(ITelegramBotClient botClient, Message message);
}