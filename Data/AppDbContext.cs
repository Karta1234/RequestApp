using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

  public DbSet<Request> Requests => Set<Request>();
  public DbSet<RequestType> RequestTypes => Set<RequestType>();
  public DbSet<RequestStatus> RequestStatuses => Set<RequestStatus>();
  public DbSet<RequestStatusHistory> RequestStatusHistories => Set<RequestStatusHistory>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
  }
}