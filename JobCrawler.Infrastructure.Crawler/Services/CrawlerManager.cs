using JobCrawler.Data.Crawler.Context;
using JobCrawler.Domain.Helpers;
using JobCrawler.Domain.Results;
using JobCrawler.Domain.Variables;
using JobCrawler.Infrastructure.Crawler.Services.Interfaces;
using JobCrawler.Services.Crawler.Services.Interfaces;
using JobCrawler.Services.TelegramAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JobCrawler.Infrastructure.Crawler.Services
{
    public class CrawlerManager : ICrawlerManager, IHostedService, IDisposable
    {
        private readonly ILogger<CrawlerManager> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IDbContextFactory<ApplicationDbContext> _context;
        private Timer _timer;

        public CrawlerManager(
            ILogger<CrawlerManager> logger,
            IServiceScopeFactory scopeFactory,
            IDbContextFactory<ApplicationDbContext> context)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _context = context;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Job Crawler Hosted Service running.");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(SharedVariables.TimeIntervalSeconds + 60));
            return Task.CompletedTask;
        }

        public async Task<Result<bool>> CrawlAndSendJobPostsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var crawlerService = scope.ServiceProvider.GetRequiredService<ICrawlerService>();
            var telegramService = scope.ServiceProvider.GetRequiredService<ITelegramService>();
            await using var dbContext = await _context.CreateDbContextAsync();

            // Retrieve jobs
            var jobs = await crawlerService.GetJobsAsync();

            // Remove the jobs with no description or with description "N/A"
            jobs.RemoveAll(job => string.IsNullOrEmpty(job.JobDescription) || job.JobDescription == "N/A");

            // Remove the jobs which are not posted within the last 30 minutes
            jobs.RemoveAll(job =>
            {
                var seconds = TimeExtractor.GetSeconds(job.PostedDate!);
                return seconds is null or > SharedVariables.TimeIntervalSeconds;
            });
            
            // Retrieve active users and their keywords
            var activeUsers = await dbContext.Users
                .Where(user => user.IsActive)
                .Include(user => user.UserKeywords)
                .ThenInclude(uk => uk.Keyword)
                .ToListAsync();

            // Send job posts to relevant users
            foreach (var job in jobs)
            {
                foreach (var user in from user in activeUsers
                         let userKeywords = user.UserKeywords
                             .Select(uk => uk.Keyword.Name)
                             .ToList()
                         let jobKeywords = job.JobDescription?
                             .Split(' ', ',', ';')
                             .Select(k => k.Trim().ToLower())
                             .ToList() ?? []
                         where jobKeywords.Any(userKeywords
                             .Contains)
                         select user)
                {
                    await telegramService.SendJobPostsAsync(job, user.ClientId);
                    await Task.Delay(1000); // Delay to avoid hitting rate limits
                }
            }
            
            // Replace .NET with DotNet and C# with CSharp in the job title and description
            jobs.ForEach(job =>
            {
                if (job.JobDescription != null)
                {
                    job.JobDescription = job.JobDescription
                        .Replace(".NET", "DotNet")
                        .Replace("C#", "CSharp");
                }
            });
            
            // Send job posts to the channel
            foreach (var job in jobs)
            {
                await telegramService.SendJobPostsAsync(job);
                await Task.Delay(1000); // Delay to avoid hitting rate limits
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

        void IDisposable.Dispose()
        {
            _timer.Dispose();
        }
    }
}