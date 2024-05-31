using JobCrawler.Domain.Results;
using JobCrawler.Infrastructure.Crawler.Services.Interfaces;
using JobCrawler.Services.Crawler.Services.Interfaces;
using JobCrawler.Services.TelegramAPI.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JobCrawler.Infrastructure.Crawler.Services
{
    public class CrawlerManager : ICrawlerManager, IHostedService, IDisposable
    {
        private readonly ILogger<CrawlerManager> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _timer;

        public CrawlerManager(
            ILogger<CrawlerManager> logger, 
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Job Crawler Hosted Service running.");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(31));
            return Task.CompletedTask;
        }
        
        public async Task<Result<bool>> CrawlAndSendJobPostsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var crawlerService = scope.ServiceProvider.GetRequiredService<ICrawlerService>();
            var telegramService = scope.ServiceProvider.GetRequiredService<ITelegramService>();

            var jobs = await crawlerService.GetJobsAsync();
            foreach (var job in jobs)
            {
                await telegramService.SendJobPostsAsync(job);
                await Task.Delay(1000);
            }
            return Result<bool>.Success(true);
        }
        
        private async void DoWork(object state)
        {
            var result = await CrawlAndSendJobPostsAsync();
            if (result.IsSuccess)
            {
                _logger.LogInformation("Jobs crawled and sent to Telegram successfully.");
            }
            else
            {
                _logger.LogError("Failed to crawl jobs or send to Telegram.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Job Crawler Hosted Service is stopping.");
            _timer.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}