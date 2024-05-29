using HtmlAgilityPack;
using JobCrawler.Services.Crawler.DTO;
using JobCrawler.Services.Crawler.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace JobCrawler.Services.Crawler.Services;

public class CrawlerService : ICrawlerService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CrawlerService> _logger;

    public CrawlerService(HttpClient httpClient, ILogger<CrawlerService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<JobDto>> GetJobsAsync(IEnumerable<string> keywords, string location)
    {
        var jobs = new List<JobDto>();
        var keywordString = string.Join("+OR+", keywords);
        var url = $"https://www.linkedin.com/jobs/search/?f_TPR=r1440&keywords={keywordString}&location={location}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 Safari/537.36");

        var response = await _httpClient.SendAsync(request);
        var pageContents = await response.Content.ReadAsStringAsync();

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(pageContents);
        
        var jobCards = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class, 'base-card') and contains(@class, 'job-search-card')]");

        if (jobCards == null) 
            return jobs;

        foreach (var card in jobCards)
        {
            var jobTitleNode = card.SelectSingleNode(".//h3[contains(@class, 'base-search-card__title')]");
            var companyNode = card.SelectSingleNode(".//h4[contains(@class, 'base-search-card__subtitle')]");
            var locationNode = card.SelectSingleNode(".//span[contains(@class, 'job-search-card__location')]");
            var jobUrlNode = card.SelectSingleNode(".//a[contains(@class, 'base-card__full-link')]");
            var postedDateNode = card.SelectSingleNode(".//time[contains(@class, 'job-search-card__listdate')]");

            var jobTitle = jobTitleNode?.InnerText.Trim() ?? "N/A";
            var companyName = companyNode?.InnerText.Trim() ?? "N/A";
            var jobLocation = locationNode?.InnerText.Trim() ?? "N/A";
            var jobUrl = jobUrlNode?.Attributes["href"]?.Value ?? "N/A";
            var postedDate = postedDateNode?.InnerText.Trim() ?? "N/A";

            jobs.Add(new JobDto
            {
                Title = jobTitle,
                Company = companyName,
                Location = jobLocation,
                Url = jobUrl,
                PostedDate = postedDate
            });
        }

        return jobs;
    }
}
