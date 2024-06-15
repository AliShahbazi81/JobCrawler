using JobCrawler.Data.Crawler.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobCrawler.Data.Crawler.Context;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Keyword> Keywords { get; set; }
    public DbSet<UserKeyword> UserKeywords { get; set; }
    public DbSet<UserCountry> UserCountries { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new CountryConfiguration());
        modelBuilder.ApplyConfiguration(new KeywordConfiguration());
        modelBuilder.ApplyConfiguration(new UserKeywordConfiguration());
        modelBuilder.ApplyConfiguration(new UserCountryConfiguration());
    }
}