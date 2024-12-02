namespace JobCrawler.Services.Crawler.Services;

public class RateLimiter
{
    private readonly SemaphoreSlim _rateLimiter = new SemaphoreSlim(1, 1);
    private readonly TimeSpan _delay;

    public RateLimiter(TimeSpan delay)
    {
        _delay = delay;
    }

    public async Task<T> PerformAsync<T>(Func<Task<T>> action)
    {
        await _rateLimiter.WaitAsync();
        try
        {
            var result = await action();
            return result;
        }
        finally
        {
            await Task.Delay(_delay);
            _rateLimiter.Release();
        }
    }

    public async Task PerformAsync(Func<Task> action)
    {
        await _rateLimiter.WaitAsync();
        try
        {
            await action();
        }
        finally
        {
            await Task.Delay(_delay);
            _rateLimiter.Release();
        }
    }
}