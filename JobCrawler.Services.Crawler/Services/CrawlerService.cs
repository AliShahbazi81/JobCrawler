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

        // Data Science, Machine Learning, Data Analysis
        "Pandas", "NumPy", "SciPy", "scikit-learn", "TensorFlow", "Keras", "PyTorch", "Matplotlib", "Seaborn", "Jupyter", "R", "RStudio", "SPSS", "SAS", "Stata", "Excel", "Tableau", "Power BI", "Hadoop", "Spark", "Hive", "Pig", "HBase", "Mahout", "MLlib", "Data Mining", "Data Wrangling", "Data Visualization", "Predictive Analytics", "Statistical Analysis", "Deep Learning", "Neural Networks", "Natural Language Processing", "Computer Vision", "Big Data", "Time Series Analysis", "Bayesian Inference", "A/B Testing", "Feature Engineering", "Model Deployment", "AutoML", "Data Cleaning", "ETL", "Dimensionality Reduction", "Clustering", "Classification", "Regression", "Machine Learning Algorithms", "Data Preprocessing", "Data Collection", "Data Warehousing",

        // Tools and Technologies
        "AWS", "Azure", "Docker", "Kubernetes", "Jenkins", "Git", "GitHub", "GitLab", "Bitbucket", "Jira", "Confluence", "Slack", "Trello", "Azure DevOps", "AWS CodePipeline", "AWS CodeBuild", "AWS CodeDeploy", "AWS CodeCommit", "AWS CodeStar", "AWS CodeArtifact", "AWS CodeGuru", "Ubuntu", "Debian", "CentOS", "RedHat", "Fedora", "Windows", "MacOS", "Linux", "Unix", "Shell Scripting", "PowerShell", "Bash", "Zsh", "Terraform", "Ansible", "Chef", "Puppet", "SaltStack", "Nginx", "Apache", "IIS", "Logstash", "Kibana", "Prometheus", "Grafana", "Splunk", "Datadog", "New Relic", "Sentry", "AppDynamics", "Dynatrace", "Postman", "Swagger", "OpenAPI", "GraphQL", "gRPC", "SOAP", "WebSockets", "WebRTC", "OAuth", "JWT", "SAML", "OpenID", "LDAP", "Active Directory",

        // Others
        "OAuth2", "OIDC", "SAML2", "OpenID2", "LDAP2", "Active Directory2", "OAuth3", "OIDC3", "SAML3", "OpenID3", "LDAP3", "Active Directory3", "OAuth4", "OIDC4", "SAML4", "OpenID4", "LDAP4", "Active Directory4", "OAuth5", "OIDC5", "SAML5", "OpenID5", "LDAP5", "Active Directory5", "OAuth6", "OIDC6", "SAML6", "OpenID6", "LDAP6", "Active Directory6", "OAuth7", "OIDC7", "SAML7", "OpenID7", "LDAP7", "Active Directory7", "OAuth8", "OIDC8", "SAML8", "OpenID8", "LDAP8", "Active Directory8", "OAuth9", "OIDC9", "SAML9", "OpenID9", "LDAP9", "Active Directory9", "OAuth10", "OIDC10", "SAML10", "OpenID10", "LDAP10", "Active Directory10", "OAuth11", "OIDC11", "SAML11", "OpenID11", "LDAP11", "Active Directory11", "OAuth12", "OIDC12", "SAML12", "OpenID12", "LDAP12", "Active Directory",

        // Civil engineering keywords
        "AutoCAD", "Civil 3D", "Revit", "Navisworks", "Tekla", "Staad Pro", "SAP2000", "ETABS", "SAFE", "Primavera", "MS Project", "Bluebeam", "SketchUp", "Rhino", "Grasshopper", "Dynamo", "ArcGIS", "QGIS", "Hec-RAS", "Hec-HMS", "SWMM", "WaterCAD", "WaterGEMS", "EPANET", "StormCAD", "SewerCAD", "AutoTURN", "Synchro", "VISSIM", "OpenRoads", "Civil Site Design", "Civil 3D", "InfraWorks", "Bentley MicroStation", "Bentley InRoads", "Bentley OpenRoads", "Bentley Descartes", "Bentley LumenRT", "Bentley ContextCapture", "Bentley Pointools", "Bentley Navigator", "Bentley ProjectWise", "Bentley AssetWise", "Bentley iTwin", "Bentley Synchro", "Bentley LEGION", "Bentley CUBE", "Bentley RAM", "Bentley SACS", "Bentley STAAD", "Bentley AutoPIPE", "Bentley ProStructures", "Bentley AECOsim", "Bentley Hevacomp", "Bentley Tas", "Bentley Amulet", "Bentley gINT", "Bentley PLAXIS", "Bentley SoilVision", "Bentley MineCycle", "Bentley MineSight", "Bentley Map", "Bentley Geo Web Publisher", "Bentley Map Mobile", "Bentley Map Enterprise", "Bentley Map PowerView", "Bentley Map PowerMap", "Bentley Map PowerCivil", "Bentley Map PowerMap Field", "Bentley Map PowerCivil Field", "Bentley Map PowerCivil Mobile", "Bentley Map PowerCivil Web", "Bentley Map PowerCivil Server", "Bentley Map PowerCivil Client", "Bentley Map PowerCivil Cloud", "Bentley Map PowerCivil Cloud Server", "Bentley Map PowerCivil Cloud Client", "Bentley Map PowerCivil Cloud Web",

        "MSP", "Microsoft Project", "Primavera P6", "MS Word", "Microsoft Excel", "BIM"
    ];

    private readonly List<string> _locations = new() { "Canada" };

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

        var baseUrl = $"https://www.linkedin.com/jobs/search/?f_TPR=r{SharedVariables.TimeIntervalSeconds}&keywords=(.NET OR Java OR HTML OR C%23 OR AWS OR Azure OR Python OR Django OR Flask OR FastAPI OR C++ OR C OR Perl OR GoLang OR CSS OR JavaScript OR React OR NextJs OR ASP.NET OR VueJs OR Angular OR NodeJs OR SQL OR NoSQL OR ExpressJs OR Laravel OR PHP OR Swift OR Android OR Ruby OR Ruby on Rails OR AutoCAD OR Civil 3D OR Civil3D OR Revit OR ETABS OR Microsoft Project OR MSP OR Primavera P6 OR P6 OR BIM OR Building Information Modeling OR Navisworks OR MS Word OR MS Excel OR Excel OR Word OR Powerpoint)&location={Uri.EscapeDataString(location)}";

        var maxRetries = 5; // Maximum retry attempts
        var backoffFactor = 2; // Exponential backoff factor

        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                // Regenerate the URL (if dynamic adjustments are needed, you can include them here)
                var refreshedUrl = baseUrl;

                // Regenerate the request for each attempt
                using var request = new HttpRequestMessage(HttpMethod.Get, refreshedUrl);
                request.Headers.Add("User-Agent", SharedVariables.UserAgent);
                request.Headers.Add("Cache-Control", "no-cache"); // Force fresh fetch

                var response = await _httpClient.SendAsync(request);

                if ((int)response.StatusCode == 429)
                {
                    Console.WriteLine($"Attempt {attempt}: Received 429 Too Many Requests. Retrying after delay...");
                    var retryAfter = response.Headers.RetryAfter?.Delta?.TotalSeconds ?? Math.Pow(backoffFactor, attempt);
                    await Task.Delay(TimeSpan.FromSeconds(retryAfter));
                    continue; // Retry the request
                }

                response.EnsureSuccessStatusCode(); // Throw if not a success status code

                var pageContents = await response.Content.ReadAsStringAsync();
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(pageContents);

                var jobCards = htmlDocument.DocumentNode.SelectNodes(SharedVariables.JobCards);

                if (jobCards == null)
                    return jobs;

                await new BrowserFetcher().DownloadAsync(Chrome.DefaultBuildId);
                var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true,
                    Timeout = 60000
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

                        // Fetch additional details for each job here (as in your original code)...
                        jobs.Add(job);
                    }
                }
                finally
                {
                    await browser.CloseAsync();
                }

                return jobs; // If we successfully complete, return the jobs
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Attempt {attempt}: Failed with error: {ex.Message}");
                if (attempt == maxRetries)
                {
                    Console.WriteLine("Max retry attempts reached. Aborting.");
                    break; // Exit the loop after max retries
                }

                var retryDelay = Math.Pow(backoffFactor, attempt); // Exponential backoff
                Console.WriteLine($"Retrying in {retryDelay} seconds...");
                await Task.Delay(TimeSpan.FromSeconds(retryDelay));
            }
        }

        return jobs; // Return whatever jobs were processed, or an empty list if no success
    }
}