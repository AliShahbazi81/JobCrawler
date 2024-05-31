using JobCrawler.Services.Crawler.DTO;

namespace JobCrawler.Services.TelegramAPI.Services.Interfaces;

public interface ITelegramService
{
    Task SendJobPostsAsync(JobDto job);
    
    Task<bool> UpdateMessageAsync(int messageId, string message);
}