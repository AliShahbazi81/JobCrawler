using JobCrawler.Data.Crawler.Context;
using JobCrawler.Services.TelegramAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = JobCrawler.Data.Crawler.Entities.User;

namespace JobCrawler.Services.TelegramAPI.Services.Commands;

public class StartCommand : IBotCommand
{
    private readonly IDbContextFactory<ApplicationDbContext> _context;

    public StartCommand(IDbContextFactory<ApplicationDbContext> context)
    {
        _context = context;
    }

    public string Command => "/start";
    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
    {
        var inlineKeyboards = new InlineKeyboardMarkup(new[]
        {
            [
                InlineKeyboardButton.WithCallbackData("\ud83d\udd0e Channels", "/channels"),
                InlineKeyboardButton.WithCallbackData("\ud83d\udde3 Discussion", "/discussion")
            ],
            new[]
            {
                InlineKeyboardButton.WithCallbackData("\ud83d\udcb3 Donation", "/donation"),
                InlineKeyboardButton.WithCallbackData("\ud83d\udcbb Contact Us", "/contact_us"),
            }
        });
        
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Welcome !\n" +
                  "Thank you for choosing us. Just for a quick heads up, this bot is in alpha version (v0.1.0) and we are continuously working on it to make it better.\n",
            replyMarkup: inlineKeyboards,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html
        );
        
        await RegisterUserAsync(message.Chat.Id, message.Chat.Username);
    }

    private async Task RegisterUserAsync(long userId, string? username)
    {
        await using var dbContext = await _context.CreateDbContextAsync();
        var user = await dbContext.Users
            .SingleOrDefaultAsync(x => x.ClientId == userId);
        
        if(user is not null)
            return;
        
        user = new User
        {
            ClientId = userId,
            Username = username,
            JoinedAt = DateTime.Now.ToUniversalTime()
        };
        
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
    }
}