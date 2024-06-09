using HtmlAgilityPack;
using JobCrawler.Domain.Variables;
using JobCrawler.Services.Crawler.DTO;
using JobCrawler.Services.Crawler.Services.Interfaces;
using Polly;
using PuppeteerSharp;
using PuppeteerSharp.BrowserData;

namespace JobCrawler.Services.Crawler.Services;

public class CrawlerService : ICrawlerService
{
    private readonly HttpClient _httpClient;

    private readonly string[] _softwareKeywords =
    [
        // Backend Development
        ".NET", "Java", "C#", "Python", "Django", "Flask", "FastAPI", "C++", "Laravel", "PHP", "Ruby", "Ruby on Rails", "Swift", "Perl", "GoLang", "NodeJs", "ExpressJs", "ASP.NET",

        // Frontend Development
        "HTML", "CSS", "JavaScript", "React", "NextJs", "Angular", "VueJs", "TypeScript",

        // Database
        "SQL", "NoSQL", "MongoDB", "PostgreSQL", "MySQL", "SQLite", "Redis", "RabbitMQ", "Kafka", "Elasticsearch",

        // Tools and Technologies
        "AWS", "Azure", "Docker", "Kubernetes", "Jenkins", "Git", "GitHub", "GitLab", "Bitbucket", "Jira", "Confluence", "Slack", "Trello", "Azure DevOps", "AWS CodePipeline", "AWS CodeBuild", "AWS CodeDeploy", "AWS CodeCommit", "AWS CodeStar", "AWS CodeArtifact", "AWS CodeGuru", "Ubuntu", "Debian", "CentOS", "RedHat", "Fedora", "Windows", "MacOS", "Linux", "Unix", "Shell Scripting", "PowerShell", "Bash", "Zsh", "Terraform", "Ansible", "Chef", "Puppet", "SaltStack", "Nginx", "Apache", "IIS", "Logstash", "Kibana", "Prometheus", "Grafana", "Splunk", "Datadog", "New Relic", "Sentry", "AppDynamics", "Dynatrace", "Postman", "Swagger", "OpenAPI", "REST", "GraphQL", "gRPC", "SOAP", "WebSockets", "WebRTC", "OAuth", "JWT", "SAML", "OpenID", "LDAP", "Active Directory",

        // Others
        "OAuth2", "OIDC", "SAML2", "OpenID2", "LDAP2", "Active Directory2", "OAuth3", "OIDC3", "SAML3", "OpenID3", "LDAP3", "Active Directory3", "OAuth4", "OIDC4", "SAML4", "OpenID4", "LDAP4", "Active Directory4", "OAuth5", "OIDC5", "SAML5", "OpenID5", "LDAP5", "Active Directory5", "OAuth6", "OIDC6", "SAML6", "OpenID6", "LDAP6", "Active Directory6", "OAuth7", "OIDC7", "SAML7", "OpenID7", "LDAP7", "Active Directory7", "OAuth8", "OIDC8", "SAML8", "OpenID8", "LDAP8", "Active Directory8", "OAuth9", "OIDC9", "SAML9", "OpenID9", "LDAP9", "Active Directory9", "OAuth10", "OIDC10", "SAML10", "OpenID10", "LDAP10", "Active Directory10", "OAuth11", "OIDC11", "SAML11", "OpenID11", "LDAP11", "Active Directory11", "OAuth12", "OIDC12", "SAML12", "OpenID12", "LDAP12", "Active Directory"
    ];

    private readonly List<string> _locations = new() { "Canada", "Germany", "Australia", "Netherlands" };

    public CrawlerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<JobDto>> GetJobsAsync()
    {
        var tasks = _locations.Select(GetJobsForLocationAsync).ToList();
        /* Since crawling jobs for different countries take times, we run them concurrently to improve performance and scalability */
        var results = await Task.WhenAll(tasks);

        /* After crawling all jobs, merge them into a single list -> To be sent as separated posts in related telegram channel */
        return results.SelectMany(result => result).ToList();
    }

    private async Task<List<JobDto>> GetJobsForLocationAsync(string location)
    {
        var delay = 3000;
        var jobs = new List<JobDto>();

        var url = $"https://www.linkedin.com/jobs/search/?f_TPR=r{SharedVariables.TimeIntervalSeconds}&keywords=(.NET OR Java OR HTML OR C%23 OR AWS OR Azure OR Python OR Django OR Flask OR FastAPI OR C++ OR C OR Perl OR GoLang OR CSS OR JavaScript OR React OR NextJs OR ASP.NET OR VueJs OR Angular OR NodeJs OR SQL OR NoSQL OR ExpressJs OR Laravel OR PHP OR Swift OR Android OR Ruby OR Ruby on Rails)&location={Uri.EscapeDataString(location)}";

        var policy = Policy.Handle<HttpRequestException>().RetryAsync(2);

        await policy.ExecuteAsync(async () =>
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", SharedVariables.UserAgent);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var pageContents = await response.Content.ReadAsStringAsync();
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(pageContents);

            var jobCards = htmlDocument.DocumentNode.SelectNodes(SharedVariables.JobCards);

            if (jobCards == null)
                return;

            await new BrowserFetcher().DownloadAsync(Chrome.DefaultBuildId);
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Timeout = 60000 // Increase timeout to 60 seconds
            });

            try
            {
                foreach (var card in jobCards)
                {
                    var jobTitleNode = card.SelectSingleNode(SharedVariables.JobTitleNode);
                    var companyNode = card.SelectSingleNode(SharedVariables.CompanyNode);
                    var locationNode = card.SelectSingleNode(SharedVariables.LocationNode);
                    var jobUrlNode = card.SelectSingleNode(SharedVariables.JobUrlNode);
                    var postedDateNode = card.SelectSingleNode(SharedVariables.PostedDateNode);

                    var jobTitle = jobTitleNode?.InnerText.Trim() ?? "N/A";
                    var companyName = companyNode?.InnerText.Trim() ?? "N/A";
                    var jobLocation = locationNode?.InnerText.Trim() ?? "N/A";
                    var jobUrl = jobUrlNode?.Attributes["href"]?.Value ?? "N/A";
                    var postedDate = postedDateNode?.InnerText.Trim() ?? "N/A";

                    var job = new JobDto
                    {
                        Title = jobTitle,
                        Company = companyName,
                        Location = jobLocation,
                        Url = jobUrl,
                        PostedDate = postedDate
                    };

                    // Fetch additional details from job details page
                    using var jobDetailsRequest = new HttpRequestMessage(HttpMethod.Get, job.Url);
                    jobDetailsRequest.Headers.Add("User-Agent", SharedVariables.UserAgent);

                    var jobDetailsResponse = await _httpClient.SendAsync(jobDetailsRequest);
                    var jobDetailsPageContents = await jobDetailsResponse.Content.ReadAsStringAsync();

                    var jobDetailsDocument = new HtmlDocument();
                    jobDetailsDocument.LoadHtml(jobDetailsPageContents);

                    // Extract employment type
                    var employmentTypeNode = jobDetailsDocument.DocumentNode.SelectSingleNode(SharedVariables.EmploymentTypeNode);
                    job.EmploymentType = employmentTypeNode?.InnerText.Trim() ?? "N/A";

                    // Extract number of employees
                    var numberOfEmployeesNode = jobDetailsDocument.DocumentNode.SelectSingleNode(SharedVariables.NumberOfEmployeesNode);
                    job.NumberOfEmployees = numberOfEmployeesNode?.InnerText.Trim() ?? "0 Applicants";

                    // Extract job description
                    var jobDescriptionFound = false;
                    var attempts = 0;

                    while (attempts < 5 && !jobDescriptionFound)
                    {
                        try
                        {
                            using (var page = await browser.NewPageAsync())
                            {
                                await page.GoToAsync(job.Url, new NavigationOptions
                                {
                                    WaitUntil = new[] { WaitUntilNavigation.Networkidle0 }
                                });

                                // Validate the page object
                                if (page == null)
                                {
                                    Console.WriteLine("Page object is null.");
                                    throw new Exception("Page object is null.");
                                }

                                // Increase timeout to 60 seconds
                                await page.WaitForSelectorAsync(SharedVariables.JobDescriptionNode, new WaitForSelectorOptions { Timeout = delay, Visible = true });

                                var jobDescription = await page.EvaluateExpressionAsync<string>(@"
                            document.querySelector('.description__text--rich').innerText
                        ");

                                // Convert job description to lowercase
                                jobDescription = jobDescription.ToLower();

                                // Check for location type keywords
                                if (jobDescription.Contains("remote"))
                                    job.LocationType = "Remote";
                                else if (jobDescription.Contains("hybrid"))
                                    job.LocationType = "Hybrid";
                                else
                                    job.LocationType = "On-Site";

                                // List to store found keywords
                                var foundKeywords = _softwareKeywords
                                    .Where(keyword => jobDescription.Contains(keyword.ToLower()))
                                    .ToList();

                                // Set job description to found keywords or N/A if none found
                                job.JobDescription = foundKeywords.Count > 0 ? string.Join(", ", foundKeywords) : "N/A";
                                jobDescriptionFound = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            attempts++;
                            if (delay < 10_000)
                                delay += 2000;
                            Console.WriteLine($"Attempt {attempts}: Failed to get job description for URL: {job.Url}");
                            Console.WriteLine(ex.Message);
                            if (attempts >= 5)
                            {
                                job.JobDescription = "N/A";
                            }
                        }
                    }

                    jobs.Add(job);
                }
            }
            finally
            {
                await browser.CloseAsync();
            }
        });

        return jobs;
    }


    private static List<string> GetFreeProxies()
    {
        return
        [
            "http://37.120.189.106:80",
            "http://38.54.71.67:80",
            "http://217.160.99.39:80",
            "http://202.61.204.51:80",
            "http://83.98.243.181:80",
            "http://85.214.195.118:80",
            "http://51.254.78.223:80",
            "http://79.116.73.5:80",
            "http://116.203.28.43:80",
            "http://37.27.81.120:80",
            "http://65.109.189.49:80",
            "http://37.27.82.72:80",
            "http://188.132.209.245:80"
        ];
    }
}