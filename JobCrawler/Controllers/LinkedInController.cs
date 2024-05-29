using JobCrawler.Services.Crawler.DTO;
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
    
    [HttpGet("GetJobs")]
    public async Task<ActionResult<List<JobDto>>> GetJobs([FromQuery] List<string> keywords, string location)
    {
        try
        {
            var result = await _crawlerService.GetJobsAsync(keywords, location);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting jobs from LinkedIn");
            throw;
        }
    }
}