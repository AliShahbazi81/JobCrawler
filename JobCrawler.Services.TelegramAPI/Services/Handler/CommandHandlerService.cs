using JobCrawler.Data.Crawler.Context;
using JobCrawler.Services.TelegramAPI.Services.Commands;
using JobCrawler.Services.TelegramAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace JobCrawler.Services.TelegramAPI.Services.Handler;

public class CommandHandlerService
{
    private readonly ITelegramBotClient _botClient;
    private readonly List<IBotCommand> _commands;
    private readonly IDbContextFactory<ApplicationDbContext> _context;

    public CommandHandlerService(ITelegramBotClient botClient, IDbContextFactory<ApplicationDbContext> context)
    {
        _botClient = botClient;
        _context = context;
        _commands = new List<IBotCommand>
        {
            new StartCommand(context),
            new ContactUsCommand(),
            new ChannelsCommand(),
            new GroupCommand(),
            new DonationCommand(),
            new CrawlerCommand(context)
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
            else
            {
                await HandleKeywordsAsync(message);
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
        if (callbackQuery.Data == "toggle_crawler")
        {
            await ToggleCrawlerAsync(callbackQuery.From.Id);
            var updatedText = await IsCrawlerActivatedAsync(callbackQuery.From.Id) ? "\u274c Disable" : "\u2705 Enable";

            await _botClient.EditMessageReplyMarkupAsync(
                chatId: callbackQuery.Message.Chat.Id,
                messageId: callbackQuery.Message.MessageId,
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(updatedText, "toggle_crawler"),
                    }
                })
            );

            await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id, "Crawler status updated.");
        }
        else
        {
            var message = new Message
            {
                Chat = callbackQuery.Message.Chat,
                MessageId = callbackQuery.Message.MessageId,
                Text = callbackQuery.Data
            };
            await HandleCommandAsync(message);
        }
    }

    private async Task HandleKeywordsAsync(Message message)
    {
        var crawlerCommand = _commands.OfType<CrawlerCommand>().FirstOrDefault();
        if (crawlerCommand != null)
        {
            await crawlerCommand.HandleKeywordsAsync(_botClient, message);
        }
    }

    private async Task ToggleCrawlerAsync(long userId)
    {
        await using var dbContext = await _context.CreateDbContextAsync();
        var user = await dbContext.Users
            .FirstOrDefaultAsync(x => x.ClientId == userId);

        if (user != null)
        {
            user.IsActive = !user.IsActive;
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task<bool> IsCrawlerActivatedAsync(long userId)
    {
        await using var dbContext = await _context.CreateDbContextAsync();
        var user = await dbContext.Users
            .FirstOrDefaultAsync(x => x.ClientId == userId);

        return user?.IsActive ?? false;
    }
}