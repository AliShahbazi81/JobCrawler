using JobCrawler.Services.Crawler.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobScrawler.Controllers;

public class LinkedInController : BaseApiController
{
    private readonly ICrawlerService _crawlerService;
    private readonly ILogger<LinkedInController> _logger;

    public LinkedInController(
        ICrawlerService crawlerService, 
        ILogger<LinkedInController> logger)
    {
        _crawlerService = crawlerService;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<ActionResult> GetJobsAsync(string keyword, string location)
    {
        try
        {
            var result = await _crawlerService.GetJobsAsync(keyword, location);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting jobs from LinkedIn");
            throw;
        }
    }
}