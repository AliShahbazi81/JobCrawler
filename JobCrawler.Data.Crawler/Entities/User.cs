using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCrawler.Data.Crawler.Entities;

public class User
{
    public int ClientId { get; set; }
    public string? Username { get; set; }
    public DateTime JoinedAt { get; set; }
    
    public virtual ICollection<UserKeyword> UserKeywords { get; set; } = new List<UserKeyword>();
    public virtual ICollection<UserField> UserFields { get; set; } = new List<UserField>();
    public virtual ICollection<UserCountry> UserCountries { get; set; } = new List<UserCountry>();
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.ClientId);
        builder.Property(u => u.ClientId).ValueGeneratedNever();
        builder.Property(u => u.Username).IsRequired().HasMaxLength(100);
        builder.Property(x => x.JoinedAt).HasDefaultValueSql("getdate()");
        builder.HasMany(u => u.UserKeywords).WithOne(uk => uk.User).HasForeignKey(uk => uk.ClientId);
        builder.HasMany(u => u.UserFields).WithOne(uf => uf.User).HasForeignKey(uf => uf.ClientId);
        builder.HasMany(u => u.UserCountries).WithOne(uc => uc.User).HasForeignKey(uc => uc.ClientId);
    }
}