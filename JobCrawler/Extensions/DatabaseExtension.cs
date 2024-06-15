using JobCrawler.Data.Crawler.Context;
using Microsoft.EntityFrameworkCore;

namespace JobScrawler.Extensions;

public static class DatabaseExtension
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPooledDbContextFactory<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("JobCrawlerDatabase")));

        return services;
    }

    public static void EnsureDatabaseMigrated(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
        var contextFactory = serviceScope?.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
        if (contextFactory == null)
            return;

        using var context = contextFactory.CreateDbContext();
        context.Database.Migrate();
    }
}