using JobCrawler.Data.Crawler.Context;
using JobCrawler.Services.TelegramAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace JobCrawler.Services.TelegramAPI.Services.Commands;

public class CrawlerCommand : IBotCommand
{
    private readonly IDbContextFactory<ApplicationDbContext> _context;

    public CrawlerCommand(IDbContextFactory<ApplicationDbContext> context)
    {
        _context = context;
    }

    public string Command => "/crawler";
    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
    {
        var fields = await GetFieldsAsync();
        var inlineKeyboards = null as InlineKeyboardMarkup;

        foreach (var field in fields)
        {
            inlineKeyboards = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(field, $"/{field}"),
                }
            });
        }
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Please select a field you want to change the settings for:",
            replyMarkup: inlineKeyboards,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html
        );
    }
    
    private async Task<List<string>> GetFieldsAsync()
    {
        await using var dbContext = await _context.CreateDbContextAsync();
        return await dbContext.Fields
            .AsNoTracking()
            .Select(x => x.Name)
            .ToListAsync();
    }
}