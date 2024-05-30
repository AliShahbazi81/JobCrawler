using JobCrawler.Services.Crawler.DTO;

namespace JobCrawler.Services.TelegramAPI.Services.Interfaces;

public interface ITelegramService
{
    Task SendJobPostsAsync(List<JobDto> jobs);
    
    Task<bool> UpdateMessageAsync(int messageId, string message);
}