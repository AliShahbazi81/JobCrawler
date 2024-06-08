using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCrawler.Data.Crawler.Entities;

public class Country
{
    public int Id { get; set; }
    public required string Name { get; set; }
    
    public ICollection<Field> Fields { get; set; }
    public ICollection<UserCountry> UserCountries { get; set; }
}

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(c => c.Name).IsUnique();
        builder.HasMany(c => c.Fields)
            .WithOne(f => f.Country)
            .HasForeignKey(f => f.CountryId);
        builder.HasMany(c => c.UserCountries)
            .WithOne(uc => uc.Country)
            .HasForeignKey(uc => uc.CountryId);
    }
}