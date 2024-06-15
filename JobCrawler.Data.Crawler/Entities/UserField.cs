using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCrawler.Data.Crawler.Entities;

public class UserField
{
    public int Id { get; set; }
    public bool IsEnabled { get; set; } = true;
    
    public long ClientId { get; set; }
    public User User { get; set; }
    public int FieldId { get; set; }
    public Field Field { get; set; }
    
}

public class UserFieldConfiguration : IEntityTypeConfiguration<UserField>
{
    public void Configure(EntityTypeBuilder<UserField> builder)
    {
        builder.HasKey(uf => new { uf.ClientId, uf.FieldId });
        builder.HasOne(uf => uf.User).WithMany(u => u.UserFields).HasForeignKey(uf => uf.ClientId);
        builder.HasOne(uf => uf.Field).WithMany(f => f.UserFields).HasForeignKey(uf => uf.FieldId);
    }
}