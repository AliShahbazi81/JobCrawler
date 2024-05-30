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
    private readonly string[] _softwareKeywords = [".NET", "Java", "HTML", "C#", "AWS", "Azure", "Python", "Django", "Flask", "FastAPI", "C++",
        "C", "Perl", "GoLang", "CSS", "JavaScript", "React", "NextJs", "ExpressJs" , "ASP.NET", "React", "NodeJs", "Angular", "VueJs", "TypeScript",
        "SQL", "NoSQL", "MongoDB", "PostgreSQL", "MySQL", "SQLite", "Docker", "Kubernetes", "Jenkins", "Git", "GitHub", "GitLab", "Bitbucket",
        "Jira", "Confluence", "Slack", "Trello", "Azure DevOps", "AWS CodePipeline", "AWS CodeBuild", "AWS CodeDeploy", "AWS CodeCommit",
        "AWS CodeStar", "AWS CodeArtifact", "AWS CodeGuru", "Ubuntu", "Debian", "CentOS", "RedHat", "Fedora", "Windows", "MacOS", "Linux",
        "Unix", "Shell Scripting", "PowerShell", "Bash", "Zsh", "Terraform", "Ansible", "Chef", "Puppet", "SaltStack", "Nginx", "Apache", "IIS",
        "Redis", "RabbitMQ", "Kafka", "Elasticsearch", "Logstash", "Kibana", "Prometheus", "Grafana", "Splunk", "Datadog", "New Relic", "Sentry", 
        "AppDynamics", "Dynatrace", "Postman", "Swagger", "OpenAPI", "REST", "GraphQL", "gRPC", "SOAP", "WebSockets", "WebRTC", "OAuth", "JWT",
        "SAML", "OpenID", "LDAP", "Active Directory", "OAuth2", "OIDC", "SAML2", "OpenID2", "LDAP2", "Active Directory2", "OAuth3", "OIDC3", 
        "SAML3", "OpenID3", "LDAP3", "Active Directory3", "OAuth4", "OIDC4", "SAML4", "OpenID4", "LDAP4", "Active Directory4", "OAuth5", "OIDC5", 
        "SAML5", "OpenID5", "LDAP5", "Active Directory5", "OAuth6", "OIDC6", "SAML6", "OpenID6", "LDAP6", "Active Directory6", "OAuth7", "OIDC7",
        "SAML7", "OpenID7", "LDAP7", "Active Directory7", "OAuth8", "OIDC8", "SAML8", "OpenID8", "LDAP8", "Active Directory8", "OAuth9", "OIDC9", 
        "SAML9", "OpenID9", "LDAP9", "Active Directory9", "OAuth10", "OIDC10", "SAML10", "OpenID10", "LDAP10", "Active Directory10", "OAuth11",
        "OIDC11", "SAML11", "OpenID11", "LDAP11", "Active Directory11", "OAuth12", "OIDC12", "SAML12", "OpenID12", "LDAP12", "Active Directory"];

    public CrawlerService(HttpClient httpClient, ILogger<CrawlerService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<JobDto>> GetJobsAsync()
    {
        var jobs = new List<JobDto>();

        const string url = "https://www.linkedin.com/jobs/search/?f_TPR=r900&keywords=(.NET OR Java OR HTML OR C%23 OR AWS OR Azure OR Python OR Django OR Flask OR FastAPI OR C++ OR C OR Perl OR GoLang OR CSS OR JavaScript OR React OR NextJs OR ASP.NET OR VueJs OR Angular OR NodeJs OR SQL OR NoSQL OR ExpressJs)&location=Canada";

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
            var employmentTypeNode = jobDetailsDocument.DocumentNode.SelectSingleNode("//span[contains(@class, 'description__job-criteria-text') and (text()='Full-time' or text()='Part-time')]");
            job.EmploymentType = employmentTypeNode?.InnerText.Trim() ?? "N/A";

            // Extract location type
            var locationTypeNode = jobDetailsDocument.DocumentNode.SelectSingleNode("//span[contains(@class, 'job-criteria__text') and (text()='Remote' or text()='On-site' or text()='Hybrid')]");
            job.LocationType = locationTypeNode?.InnerText.Trim() ?? "N/A";

            // Extract number of employees
            var numberOfEmployeesNode = jobDetailsDocument.DocumentNode.SelectSingleNode("//span[contains(@class, 'num-applicants__caption')]");
            job.NumberOfEmployees = numberOfEmployeesNode?.InnerText.Trim() ?? "N/A";

            // Extract job description
            using (var page = await browser.NewPageAsync())
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

                        // Convert job description to lowercase
                        jobDescription = jobDescription.ToLower();

                        // List to store found keywords
                        var foundKeywords = _softwareKeywords
                            .Where(keyword => jobDescription.Contains(keyword.ToLower()))
                            .ToList();

                        // Set job description to found keywords or N/A if none found
                        job.JobDescription = foundKeywords.Count > 0 ? string.Join(", ", foundKeywords) : "N/A";
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


