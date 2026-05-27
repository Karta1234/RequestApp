using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RequestStatusConfiguration : IEntityTypeConfiguration<RequestStatus>
  {
    public void Configure(EntityTypeBuilder<RequestStatus> builder)
    {
      builder.Property(x => x.Name).IsRequired().HasMaxLength(64);
      builder.HasData(
        new RequestStatus { Id = 1, Name = "New",      IsTerminal = false },
        new RequestStatus { Id = 2, Name = "InReview", IsTerminal = false },
        new RequestStatus { Id = 3, Name = "Approved", IsTerminal = true  },
        new RequestStatus { Id = 4, Name = "Rejected", IsTerminal = true  }
      );
    }

}