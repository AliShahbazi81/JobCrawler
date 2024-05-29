using JobCrawler.Services.Crawler.DTO;

namespace JobCrawler.Services.Crawler.Services.Interfaces;

public interface ICrawlerService
{
    Task<List<JobDto>> GetJobsAsync();
}