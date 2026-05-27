using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RequestConfiguration : IEntityTypeConfiguration<Request>
{
  public void Configure(EntityTypeBuilder<Request> builder)
  {
    builder.HasKey(x => x.Id);
    builder.Property(x => x.Reason).IsRequired().HasMaxLength(2000);
    builder.HasIndex(x => new { x.EmployeeId, x.TypeId })
           .HasFilter("is_active = true")
           .IsUnique();
  }
}