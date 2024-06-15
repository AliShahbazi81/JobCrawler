using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCrawler.Data.Crawler.Entities;

public class UserCountry
{
    public int Id { get; set; }
    
    public long ClientId { get; set; }
    public User User { get; set; }
    public int CountryId { get; set; }
    public Country Country { get; set; }
    
}

public class UserCountryConfiguration : IEntityTypeConfiguration<UserCountry>
{
    public void Configure(EntityTypeBuilder<UserCountry> builder)
    {
        builder.HasKey(uc => new { uc.ClientId, uc.CountryId });
        builder.HasOne(uc => uc.User)
            .WithMany(u => u.UserCountries)
            .HasForeignKey(uc => uc.ClientId);
        builder.HasOne(uc => uc.Country).WithMany(c => c.UserCountries).HasForeignKey(uc => uc.CountryId);
    }
}