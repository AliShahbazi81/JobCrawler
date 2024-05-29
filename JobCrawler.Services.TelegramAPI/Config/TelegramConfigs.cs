namespace JobCrawler.Services.TelegramAPI.Config;

public record TelegramConfigs
{
    public required string ApiToken { get; set; }
    public required string Username { get; set; }
    public required string Channel { get; set; }
    public required string SoftwareDevelopmentChannelId { get; set; }
}