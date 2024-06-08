using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCrawler.Data.Crawler.Entities;

public class Field
{
    public int Id { get; set; }
    public required string Name { get; set; }
    
    public int CountryId { get; set; }
    public Country Country { get; set; }
    
    public virtual ICollection<Keyword> Keywords { get; set; }
    public virtual ICollection<UserField> UserFields { get; set; }
}

public class FieldConfiguration : IEntityTypeConfiguration<Field>
{
    public void Configure(EntityTypeBuilder<Field> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(f => f.Name).IsUnique();
        builder.HasOne(f => f.Country).WithMany(c => c.Fields).HasForeignKey(f => f.CountryId);
        builder.HasMany(f => f.Keywords).WithOne(k => k.Field).HasForeignKey(k => k.FieldId);
        builder.HasMany(f => f.UserFields).WithOne(uf => uf.Field).HasForeignKey(uf => uf.FieldId);
    }
}