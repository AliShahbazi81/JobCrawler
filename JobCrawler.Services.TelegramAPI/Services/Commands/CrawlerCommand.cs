using JobCrawler.Data.Crawler.Context;
using JobCrawler.Data.Crawler.Entities;
using JobCrawler.Services.TelegramAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = JobCrawler.Data.Crawler.Entities.User;

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
        var userKeywords = await GetKeywordsAsync(message.Chat.Id);
        var keywords = userKeywords.Aggregate(string.Empty, (current, t) => current + "• " + t + "\n");

        var inlineKeyboards = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(await IsCrawlerActivatedAsync(message.Chat.Id) ? "\u274c Disable" : "\u2705 Enable", "toggle_crawler"),
            }
        });

        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "\u2705 Here you can see a list of keywords that are used to send you jobs. You can add or remove keywords from the list.\n\n" +
                  "\u2705 Unique keywords will be added to list and repeated keywords will be removed from the list.\n" +
                  "\u2705 Separate the words using ','. e.g: C#, .NET, Java, Python, etc.\n\n" +
                  "\u2705 You can Active or Disable the bot using the button below.\n\n" +
                  "\u2b50\ufe0f Here are the list of keywords you have added:\n" +
                  "<pre>" + keywords + "</pre>",
            replyMarkup: inlineKeyboards,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html
        );
    }

    public async Task HandleKeywordsAsync(ITelegramBotClient botClient, Message message)
    {
        Console.WriteLine("Processing keywords: " + message.Text); // Debug statement
        var inputKeywords = message.Text.ToLower().Replace(" ", "").Split(',');

        await using var dbContext = await _context.CreateDbContextAsync();
        var dbKeywords = await dbContext.Keywords.Select(k => k.Name).ToListAsync();
        var notFoundKeywords = inputKeywords.Where(k => !dbKeywords.Contains(k)).ToList();

        if (notFoundKeywords.Count != 0)
        {
            var notFoundMessage = $"The following keywords were not found: {string.Join(", ", notFoundKeywords.Select(k => $"*{k}*"))}";
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: notFoundMessage,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
            );
        }
        else
        {
            var userId = message.Chat.Id;
            var user = await dbContext.Users.Include(u => u.UserKeywords).FirstOrDefaultAsync(u => u.ClientId == userId);

            if (user == null)
            {
                user = new User { ClientId = userId, UserKeywords = new List<UserKeyword>() };
                dbContext.Users.Add(user);
            }

            foreach (var keyword in inputKeywords)
            {
                var dbKeyword = await dbContext.Keywords.FirstOrDefaultAsync(k => k.Name == keyword);
                if (dbKeyword != null)
                {
                    user.UserKeywords.Add(new UserKeyword { User = user, Keyword = dbKeyword });
                }
            }

            await dbContext.SaveChangesAsync();

            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Keywords have been successfully added to your list."
            );
        }
    }

    private async Task<List<string>> GetKeywordsAsync(long userId)
    {
        await using var dbContext = await _context.CreateDbContextAsync();
        var keywords = await dbContext.UserKeywords
            .Include(x => x.Keyword)
            .AsNoTracking()
            .Where(x => x.ClientId == userId)
            .Select(x => x.Keyword.Name)
            .ToListAsync();

        return keywords;
    }

    private async Task<bool> IsCrawlerActivatedAsync(long userId)
    {
        await using var dbContext = await _context.CreateDbContextAsync();
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ClientId == userId);

        return user?.IsActive ?? false;
    }
}