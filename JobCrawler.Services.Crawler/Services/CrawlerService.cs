using HtmlAgilityPack;
using JobCrawler.Services.Crawler.DTO;
using JobCrawler.Services.Crawler.Services.Interfaces;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using PuppeteerSharp.BrowserData;

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

    public async Task<List<JobDto>> GetJobsAsync()
    {
        var jobs = new List<JobDto>();

        const string url = "https://www.linkedin.com/jobs/search/?f_TPR=r900&keywords=(.NET OR Java OR HTML OR C%23 OR AWS OR Azure OR Python OR Django OR Flask OR FastAPI OR C++ OR C OR Perl OR GoLang OR CSS OR JavaScript OR React OR NextJs OR ASP.NET)&location=Canada";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 Safari/537.36");

        var response = await _httpClient.SendAsync(request);
        var pageContents = await response.Content.ReadAsStringAsync();

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(pageContents);

        var jobCards = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class, 'base-card') and contains(@class, 'job-search-card')]");

        if (jobCards == null)
            return jobs;
        
        await new BrowserFetcher().DownloadAsync(Chrome.DefaultBuildId);
        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });

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

            var job = new JobDto()
            {
                Title = jobTitle,
                Company = companyName,
                Location = jobLocation,
                Url = jobUrl,
                PostedDate = postedDate
            };

            // Fetch additional details from job details page
            var jobDetailsRequest = new HttpRequestMessage(HttpMethod.Get, job.Url);
            jobDetailsRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 Safari/537.36");

            var jobDetailsResponse = await _httpClient.SendAsync(jobDetailsRequest);
            var jobDetailsPageContents = await jobDetailsResponse.Content.ReadAsStringAsync();

            var jobDetailsDocument = new HtmlDocument();
            jobDetailsDocument.LoadHtml(jobDetailsPageContents);

            // Extract employment type
            var employmentTypeNode = jobDetailsDocument.DocumentNode.SelectSingleNode("//span[contains(@class, 'job-criteria__text') and (text()='Full-time' or text()='Part-time')]");
            job.EmploymentType = employmentTypeNode?.InnerText.Trim() ?? "N/A";

            // Extract location type
            var locationTypeNode = jobDetailsDocument.DocumentNode.SelectSingleNode("//span[contains(@class, 'job-criteria__text') and (text()='Remote' or text()='On-site' or text()='Hybrid')]");
            job.LocationType = locationTypeNode?.InnerText.Trim() ?? "N/A";

            // Extract number of employees
            var numberOfEmployeesNode = jobDetailsDocument.DocumentNode.SelectSingleNode("//span[contains(@class, 'num-applicants__caption')]");
            job.NumberOfEmployees = numberOfEmployeesNode?.InnerText.Trim() ?? "N/A";

            // Extract job description
            await using (var page = await browser.NewPageAsync())
            {
                var jobDescriptionFound = false;
                var attempts = 0;
                while (attempts < 3 && !jobDescriptionFound)
                {
                    try
                    {
                        await page.GoToAsync(job.Url);

                        // Increase timeout to 60 seconds
                        await page.WaitForSelectorAsync(".description__text--rich", new WaitForSelectorOptions { Timeout = 3000, Visible = true });

                        var jobDescription = await page.EvaluateExpressionAsync<string>(@"
                        document.querySelector('.description__text--rich .show-more-less-html__markup').innerText
                    ");

                        job.JobDescription = jobDescription.Trim();
                        jobDescriptionFound = true;
                    }
                    catch (Exception ex)
                    {
                        attempts++;
                        Console.WriteLine($"Attempt {attempts}: Failed to get job description for URL: {job.Url}");
                        Console.WriteLine(ex.Message);
                        if (attempts >= 3)
                        {
                            job.JobDescription = "N/A";
                        }
                    }
                }
            }

            jobs.Add(job);
        }
        
        await browser.CloseAsync();
        return jobs;
    }
}


