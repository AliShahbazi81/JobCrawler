namespace JobCrawler.Services.TelegramAPI.Services.Interfaces;

public interface ITelegramService
{
    Task<int> SendAsync(string message);
    
    Task<bool> UpdateMessageAsync(int messageId, string message);
}