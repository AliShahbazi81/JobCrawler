namespace JobCrawler.Services.Crawler.DTO;

public record JobDto
{
    public string? Title { get; set; }
    public string? Company { get; set; }
    public string? Location { get; set; }
    public string? Url { get; set; }
    public string? PostedDate { get; set; }
    public string? EmploymentType { get; set; }
    public string? LocationType { get; set; }
    public string? NumberOfEmployees { get; set; }
    public string? JobDescription { get; set; }
}