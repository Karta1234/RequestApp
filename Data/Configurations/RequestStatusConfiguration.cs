using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RequestStatusConfiguration : IEntityTypeConfiguration<RequestStatus>
  {
    public void Configure(EntityTypeBuilder<RequestStatus> builder)
    {
      builder.Property(x => x.Name).IsRequired().HasMaxLength(64);
      builder.HasData(RequestStatuses.All);
    }

}