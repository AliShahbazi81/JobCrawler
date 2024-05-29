namespace JobCrawler.Services.Crawler.DTO;

public record JobDto
{
    public string? Title { get; set; }
    public string? Company { get; set; }
    public string? Location { get; set; }
    public string? Url { get; set; }
}