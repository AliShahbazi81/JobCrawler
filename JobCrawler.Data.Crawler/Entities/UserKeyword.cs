using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCrawler.Data.Crawler.Entities;

public class UserKeyword
{
    public int Id { get; set; }
    
    public int ClientId { get; set; }
    public User User { get; set; }
    public int KeywordId { get; set; }
    public Keyword Keyword { get; set; }
    
}

public class UserKeywordConfiguration : IEntityTypeConfiguration<UserKeyword>
{
    public void Configure(EntityTypeBuilder<UserKeyword> builder)
    {
        builder.HasKey(uk => new { uk.ClientId, uk.KeywordId });
        builder.HasOne(uk => uk.User).WithMany(u => u.UserKeywords).HasForeignKey(uk => uk.ClientId);
        builder.HasOne(uk => uk.Keyword).WithMany().HasForeignKey(uk => uk.KeywordId);
    }
}