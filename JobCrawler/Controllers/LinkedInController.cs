using JobCrawler.Infrastructure.Crawler.Services.Interfaces;
using JobCrawler.Services.Crawler.DTO;
using Microsoft.AspNetCore.Mvc;

namespace JobScrawler.Controllers;

public class LinkedInController : BaseApiController
{
    private readonly ICrawlerManager _crawlerManager;
    private readonly ILogger<LinkedInController> _logger;

    public LinkedInController(
        ILogger<LinkedInController> logger, 
        ICrawlerManager crawlerManager)
    {
        _logger = logger;
        _crawlerManager = crawlerManager;
    }
    
    [HttpGet("GetJobs")]
    public async Task<ActionResult<List<JobDto>>> GetJobs()
    {
        try
        {
            return HandleResult(await _crawlerManager.CrawlAndSendJobPostsAsync());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting jobs from LinkedIn");
            throw;
        }
    }
}