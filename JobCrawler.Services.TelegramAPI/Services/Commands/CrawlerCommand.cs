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
        // Get the list of keywords associated with the user
        var userKeywords = await GetKeywordsAsync(message.Chat.Id);

        // Aggregate the keywords into a single string with each keyword prefixed by a bullet point
        var keywords = userKeywords.Aggregate(string.Empty, (current, t) => current + "• " + t + "\n");

        // Create the inline keyboard with a single button that toggles the crawler state
        var inlineKeyboards = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                // Determine the text and callback data based on the crawler's current state
                InlineKeyboardButton.WithCallbackData(await IsCrawlerActivatedAsync(message.Chat.Id) ? "\u274c Disable" : "\u2705 Enable", "toggle_crawler"),
            }
        });

        // Send the message to the user with the list of keywords and the inline keyboard
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "\u2705 Here you can see a list of keywords that are used to send you jobs. You can add or remove keywords from the list.\n\n" +
                  "\u2705 Unique keywords will be added to list and repeated keywords will be removed from the list.\n" +
                  "\u2705 Separate the words using ','. e.g: C#, .NET, Java, Python, etc.\n\n" +
                  "\u2705 You can Active or Disable the bot using the button below.\n\n" +
                  "\u2b50\ufe0f Here are the list of keywords you have added:\n" +
                  "<pre>" + keywords + "</pre>", // Display the aggregated keywords list
            replyMarkup: inlineKeyboards, // Attach the inline keyboard
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html // Use HTML formatting in the message
        );
    }

    public async Task HandleKeywordsAsync(ITelegramBotClient botClient, Message message)
    {
        // Convert input keywords to lowercase, remove spaces, and split them by comma
        var inputKeywords = message.Text.ToLower().Replace(" ", "").Split(',');

        await using var dbContext = await _context.CreateDbContextAsync();
        // Get the list of all keywords in the database
        var dbKeywords = await dbContext.Keywords.Select(k => k.Name).ToListAsync();
        // Find keywords that are not in the database
        var notFoundKeywords = inputKeywords.Where(k => !dbKeywords.Contains(k)).ToList();

        // If there are any keywords not found in the database, inform the user
        if (notFoundKeywords.Count != 0)
        {
            var notFoundMessage = $"The following keywords were not found: {string.Join(", ", notFoundKeywords.Select(k => $"*{k}*"))}";
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: notFoundMessage,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
            );
            return;
        }

        var userId = message.Chat.Id;
        // Get the user and include their keywords
        var user = await dbContext.Users.Include(u => u.UserKeywords).ThenInclude(uk => uk.Keyword).FirstOrDefaultAsync(u => u.ClientId == userId);

        if (user == null)
        {
            user = new User { ClientId = userId, UserKeywords = new List<UserKeyword>() };
            dbContext.Users.Add(user);
        }

        // Get the list of keywords already associated with the user
        var existingUserKeywords = user.UserKeywords.Select(uk => uk.Keyword.Name).ToList();
        // Determine which keywords to add and which to remove
        var keywordsToAdd = inputKeywords.Where(k => !existingUserKeywords.Contains(k)).ToList();
        var keywordsToRemove = inputKeywords.Where(k => existingUserKeywords.Contains(k)).ToList();

        // Add new keywords to the user
        foreach (var keyword in keywordsToAdd)
        {
            var dbKeyword = await dbContext.Keywords.FirstOrDefaultAsync(k => k.Name == keyword);
            if (dbKeyword != null)
            {
                user.UserKeywords.Add(new UserKeyword { User = user, Keyword = dbKeyword });
            }
        }

        // Remove existing keywords from the user
        foreach (var keyword in keywordsToRemove)
        {
            var userKeyword = user.UserKeywords.FirstOrDefault(uk => uk.Keyword.Name == keyword);
            if (userKeyword != null)
            {
                dbContext.UserKeywords.Remove(userKeyword);
            }
        }

        // Save changes to the database
        await dbContext.SaveChangesAsync();

        // Inform the user that keywords have been updated
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Keywords have been successfully updated."
        );
    }

    private async Task<List<string>> GetKeywordsAsync(long userId)
    {
        await using var dbContext = await _context.CreateDbContextAsync();
        // Get the list of keywords associated with the user
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
        // Check if the crawler is activated for the user
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ClientId == userId);

        return user?.IsActive ?? false;
    }
}