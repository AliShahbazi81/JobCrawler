using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCrawler.Data.Crawler.Entities;

public class Keyword
{
    public int Id { get; set; }
    public required string Name { get; set; }
    
    public int FieldId { get; set; }
    public Field Field { get; set; }
    
}

public class KeywordConfiguration : IEntityTypeConfiguration<Keyword>
{
    public void Configure(EntityTypeBuilder<Keyword> builder)
    {
        builder.HasKey(k => k.Id);
        builder.Property(k => k.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(k => k.Name).IsUnique();
        builder.HasOne(k => k.Field)
            .WithMany(f => f.Keywords)
            .HasForeignKey(k => k.FieldId);
    }
}