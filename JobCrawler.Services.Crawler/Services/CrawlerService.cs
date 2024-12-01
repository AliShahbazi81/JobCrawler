using HtmlAgilityPack;
using JobCrawler.Domain.Variables;
using JobCrawler.Services.Crawler.DTO;
using JobCrawler.Services.Crawler.Services.Interfaces;

namespace JobCrawler.Services.Crawler.Services;

public class CrawlerService : ICrawlerService
{
    private readonly HttpClient _httpClient;

    private readonly string[] _softwareKeywords =
    [
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
        var allJobs = new List<JobDto>();
        var semaphore = new SemaphoreSlim(2); // Limit concurrency to 3 tasks
        var tasks = new List<Task>();

        foreach (var location in _locations)
        {
            // Wait for an available slot
            await semaphore.WaitAsync();

            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    Console.WriteLine($"Starting job crawl for location: {location}");
                    var jobs = await GetJobsForLocationAsync(location);

                    if (jobs != null)
                    {
                        lock (allJobs) // Ensure thread-safe access to the shared list
                        {
                            allJobs.AddRange(jobs);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while processing location {location}: {ex.Message}");
                }
                finally
                {
                    semaphore.Release(); // Release the slot
                }
            }));
        }

        await Task.WhenAll(tasks);

        return allJobs;
    }

    private async Task<List<JobDto>> GetJobsForLocationAsync(string location)
{
    var jobs = new List<JobDto>();

    var url = $"https://www.linkedin.com/jobs/search/?f_TPR=r{SharedVariables.TimeIntervalSeconds}&keywords=(Civil Engineer OR Civil OR Civil engineering)&location={Uri.EscapeDataString(location)}";

    try
    {
        // Fetch the job listings page
        using var initialRequest = new HttpRequestMessage(HttpMethod.Get, url);
        initialRequest.Headers.Add("User-Agent", SharedVariables.UserAgent);

        var initialResponse = await _httpClient.SendAsync(initialRequest);
        initialResponse.EnsureSuccessStatusCode();

        var pageContents = await initialResponse.Content.ReadAsStringAsync();
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(pageContents);

        var jobCards = htmlDocument.DocumentNode.SelectNodes(SharedVariables.JobCards);

        if (jobCards == null)
        {
            Console.WriteLine("No job cards found.");
            return jobs;
        }

        Console.WriteLine($"Found {jobCards.Count} jobs. Processing each job sequentially...");

        foreach (var card in jobCards)
        {
            var job = await ProcessSingleJobAsync(card);
            if (job != null)
            {
                jobs.Add(job);
            }

            // Introduce a delay to avoid sending requests too quickly
            await Task.Delay(2000); // Adjust this delay as needed
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error occurred while fetching jobs for location '{location}': {ex.Message}");
    }

    return jobs;
}

private async Task<JobDto> ProcessSingleJobAsync(HtmlNode card)
{
    try
    {
        // Extract job details from the card
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

        // Fetch additional details from the job details page
        if (!string.IsNullOrEmpty(job.Url) && Uri.IsWellFormedUriString(job.Url, UriKind.Absolute))
        {
            Console.WriteLine($"Fetching details for job: {job.Title} at {job.Company}");

            using var jobDetailsRequest = new HttpRequestMessage(HttpMethod.Get, job.Url);
            jobDetailsRequest.Headers.Add("User-Agent", SharedVariables.UserAgent);

            var jobDetailsResponse = await _httpClient.SendAsync(jobDetailsRequest);

            if ((int)jobDetailsResponse.StatusCode == 429)
            {
                Console.WriteLine("429 Too Many Requests encountered. Retrying after delay...");
                await Task.Delay(5000); // Wait before retrying
                return null; // Skip this job if still rate-limited
            }

            jobDetailsResponse.EnsureSuccessStatusCode();

            var jobDetailsPageContents = await jobDetailsResponse.Content.ReadAsStringAsync();
            var jobDetailsDocument = new HtmlDocument();
            jobDetailsDocument.LoadHtml(jobDetailsPageContents);

            // Extract employment type
            var employmentTypeNode = jobDetailsDocument.DocumentNode.SelectSingleNode(SharedVariables.EmploymentTypeNode);
            job.EmploymentType = employmentTypeNode?.InnerText.Trim() ?? "N/A";

            // Extract number of employees
            var numberOfEmployeesNode = jobDetailsDocument.DocumentNode.SelectSingleNode(SharedVariables.NumberOfEmployeesNode);
            job.NumberOfEmployees = numberOfEmployeesNode?.InnerText.Trim() ?? "0 Applicants";
        }

        // Optionally process job description here (e.g., using Puppeteer)

        return job;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error processing job card: {ex.Message}");
        return null;
    }
}

}