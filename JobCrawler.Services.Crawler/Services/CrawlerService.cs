using HtmlAgilityPack;
using JobCrawler.Services.Crawler.DTO;
using JobCrawler.Services.Crawler.Services.Interfaces;

namespace JobCrawler.Services.Crawler.Services;

public class CrawlerService : ICrawlerService
{
    private readonly HttpClient _httpClient;

    public CrawlerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<JobDto>> GetJobsAsync(string keyword, string location)
    {
        var jobs = new List<JobDto>();
        var url = $"https://www.linkedin.com/jobs/search/?keywords={keyword}&location={location}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 Safari/537.36");

        var response = await _httpClient.SendAsync(request);
        var pageContents = await response.Content.ReadAsStringAsync();

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(pageContents);

        var jobCards = htmlDocument.DocumentNode.SelectNodes("//div[@class='result-card__contents']");

        if (jobCards == null) 
            return jobs;

        jobs.AddRange(from card in jobCards let jobTitle = 
            card.SelectSingleNode(".//h3[@class='result-card__title']")
                .InnerText.Trim() let companyName = card.SelectSingleNode(".//h4[@class='result-card__subtitle']")
            .InnerText.Trim() let jobLocation = card.SelectSingleNode(".//span[@class='job-result-card__location']")
            .InnerText.Trim() let jobUrl = card.SelectSingleNode(".//a[@class='result-card__full-card-link']")
            .Attributes["href"].Value select new JobDto
        {
            Title = jobTitle, 
            Company = companyName, 
            Location = jobLocation, 
            Url = jobUrl
        });

        return jobs;
    }
}
