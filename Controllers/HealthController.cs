using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/health")]
public class HealthController: ControllerBase
{
  private readonly AppDbContext _db;
  public HealthController(AppDbContext db) => _db = db;

  [HttpGet]
  [Authorize]
  public async Task<IActionResult> Get(CancellationToken ct)
  {
    var canConnect = await _db.Database.CanConnectAsync(ct);
    return canConnect ? Ok(new {status = "Healthy", db = "up"}) : StatusCode(503, new {status= "Unhealty", db = "down"});
  }

}