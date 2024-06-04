namespace JobCrawler.Services.TelegramAPI.Config;

public record StripeConfig
{
    public required string PaymentLink { get; set; }
}