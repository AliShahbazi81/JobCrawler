using JobCrawler.Domain.Results;

namespace JobCrawler.Infrastructure.Crawler.Services.Interfaces;

public interface ICrawlerManager
{
    Task<Result<bool>> CrawlAndSendJobPostsAsync();
}