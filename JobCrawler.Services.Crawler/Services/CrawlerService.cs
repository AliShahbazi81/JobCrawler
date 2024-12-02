using System.Net;
using HtmlAgilityPack;
using JobCrawler.Domain.Variables;
using JobCrawler.Services.Crawler.DTO;
using JobCrawler.Services.Crawler.Services.Interfaces;
using Polly.Retry;
using Polly;

namespace JobCrawler.Services.Crawler.Services;

public class CrawlerService : ICrawlerService
{
    private readonly HttpClient _httpClient;

    private readonly string[] _softwareKeywords =
    [
        // Civil engineering keywords
        "AutoCAD 3D", "AutoCad 2D", "Civil 3D", "Revit", "Navisworks", "Tekla", "Staad Pro", "SAP2000", "ETABS", "SAFE", "Primavera", "MS Project", "Bluebeam", "SketchUp", "Rhino", "Grasshopper", "Dynamo", "ArcGIS", "QGIS", "Hec-RAS", "Hec-HMS", "SWMM", "WaterCAD", "WaterGEMS", "EPANET", "StormCAD", "SewerCAD", "AutoTURN", "Synchro", "VISSIM", "OpenRoads", "Civil Site Design", "Civil 3D", "InfraWorks", "Bentley MicroStation", "Bentley InRoads", "Bentley OpenRoads", "Bentley Descartes", "Bentley LumenRT", "Bentley ContextCapture", "Bentley Pointools", "Bentley Navigator", "Bentley ProjectWise", "Bentley AssetWise", "Bentley iTwin", "Bentley Synchro", "Bentley LEGION", "Bentley CUBE", "Bentley RAM", "Bentley SACS", "Bentley STAAD", "Bentley AutoPIPE", "Bentley ProStructures", "Bentley AECOsim", "Bentley Hevacomp", "Bentley Tas", "Bentley Amulet", "Bentley gINT", "Bentley PLAXIS", "Bentley SoilVision", "Bentley MineCycle", "Bentley MineSight", "Bentley Map", "Bentley Geo Web Publisher", "Bentley Map Mobile", "Bentley Map Enterprise", "Bentley Map PowerView", "Bentley Map PowerMap", "Bentley Map PowerCivil", "Bentley Map PowerMap Field", "Bentley Map PowerCivil Field", "Bentley Map PowerCivil Mobile", "Bentley Map PowerCivil Web", "Bentley Map PowerCivil Server", "Bentley Map PowerCivil Client", "Bentley Map PowerCivil Cloud", "Bentley Map PowerCivil Cloud Server", "Bentley Map PowerCivil Cloud Client", "Bentley Map PowerCivil Cloud Web",

        "MSP", "Microsoft Project", "Primavera P6", "MS Word", "Microsoft Excel", "BIM", "P6"
    ];

    private readonly List<string> _locations = new() { "Canada" };

    // Global delay to respect Retry-After headers
    private static TimeSpan _globalDelay = TimeSpan.Zero;

    // Retry policy for handling 429 errors
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

    // Semaphore to control concurrency
    private readonly SemaphoreSlim _semaphore;

    // Rate limiter to control the rate of requests
    private readonly RateLimiter _rateLimiter;

    public CrawlerService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        // Initialize the retry policy
        _retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => r.StatusCode == (HttpStatusCode)429)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: (retryAttempt, response, context) =>
                {
                    if (response.Result.Headers.TryGetValues("Retry-After", out var values))
                    {
                        if (int.TryParse(values.First(), out int seconds))
                        {
                            _globalDelay = TimeSpan.FromSeconds(seconds);
                            return _globalDelay;
                        }
                    }

                    _globalDelay = TimeSpan.FromSeconds(10); // Default delay
                    return _globalDelay;
                },
                onRetryAsync: async (response, timespan, retryAttempt, context) =>
                {
                    Console.WriteLine($"Received 429. Applying global delay of {timespan}. Retry attempt {retryAttempt}.");
                    await Task.CompletedTask;
                }
            );

        // Initialize the semaphore for concurrency control (set to 1 for sequential processing)
        _semaphore = new SemaphoreSlim(1);

        // Initialize the rate limiter with a delay between requests
        _rateLimiter = new RateLimiter(TimeSpan.FromSeconds(2));
    }

    public async Task<List<JobDto>> GetJobsAsync()
    {
        var allJobs = new List<JobDto>();
        var tasks = new List<Task>();

        foreach (var location in _locations)
        {
            // Wait for an available slot
            await _semaphore.WaitAsync();

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
                    // Introduce a delay before releasing the semaphore to space out tasks
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    _semaphore.Release();
                }
            }));
        }

        await Task.WhenAll(tasks);

        return allJobs;
    }

    private async Task<List<JobDto>> GetJobsForLocationAsync(string location)
    {
        var jobs = new List<JobDto>();

        var url = $"https://www.linkedin.com/jobs/search/?f_TPR=r{SharedVariables.TimeIntervalSeconds}&keywords=(Civil Engineer OR Civil OR Civil engineering OR Junior civil engineer OR Project coordinator OR Field engineer OR Civil engineer OR Estimation engineer)&location={Uri.EscapeDataString(location)}";

        try
        {
            // Apply global delay if set
            if (_globalDelay > TimeSpan.Zero)
            {
                Console.WriteLine($"Waiting for {_globalDelay.TotalSeconds} seconds due to previous 429 response.");
                await Task.Delay(_globalDelay);
                _globalDelay = TimeSpan.Zero; // Reset after waiting
            }

            // Fetch the job listings page with retry policy
            var initialResponse = await _retryPolicy.ExecuteAsync(async () =>
            {
                // Use the rate limiter to control the request rate
                return await _rateLimiter.PerformAsync(async () =>
                {
                    using var initialRequest = new HttpRequestMessage(HttpMethod.Get, url);
                    initialRequest.Headers.Add("User-Agent", SharedVariables.UserAgent);
                    return await _httpClient.SendAsync(initialRequest);
                });
            });

            if (!initialResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to fetch job listings for {location} after retries.");
                return jobs; // Skip this location after retries
            }

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
                else
                {
                    Console.WriteLine("Job processing returned null. Continuing to next job.");
                }

                // Introduce a delay to avoid sending requests too quickly
                await Task.Delay(TimeSpan.FromSeconds(2)); // Adjust this delay as needed
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

                // Apply global delay if set
                if (_globalDelay > TimeSpan.Zero)
                {
                    Console.WriteLine($"Waiting for {_globalDelay.TotalSeconds} seconds due to previous 429 response.");
                    await Task.Delay(_globalDelay);
                    _globalDelay = TimeSpan.Zero; // Reset after waiting
                }

                var jobDetailsResponse = await _retryPolicy.ExecuteAsync(async () =>
                {
                    // Use the rate limiter to control the request rate
                    return await _rateLimiter.PerformAsync(async () =>
                    {
                        using var jobDetailsRequest = new HttpRequestMessage(HttpMethod.Get, job.Url);
                        jobDetailsRequest.Headers.Add("User-Agent", SharedVariables.UserAgent);
                        return await _httpClient.SendAsync(jobDetailsRequest);
                    });
                });

                if (!jobDetailsResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to fetch job details for {job.Title} after retries.");
                    return null; // Skip this job after retries
                }

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

            // Optionally process job description here

            return job;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing job card: {ex.Message}");
            return null;
        }
    }
}