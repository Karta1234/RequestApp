using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

public class HeaderAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
  public HeaderAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder) {}
  protected override Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    if (!Request.Headers.TryGetValue("X-User-Id", out var userIdHeader) || !int.TryParse(userIdHeader, out var userId))
    {
      return Task.FromResult(AuthenticateResult.Fail("Missing or invalid id"));
    }
    if (!Request.Headers.TryGetValue("X-Role", out var roleHeader)  || !Enum.TryParse<Role>(roleHeader, ignoreCase: true, out var role))
    {
      return Task.FromResult(AuthenticateResult.Fail("Missing or invalid role"));
    }
    var claims = new[]
    {
      new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
      new Claim(ClaimTypes.Role, role.ToString())
    };
    var identity = new ClaimsIdentity(claims, Scheme.Name);
    var principal = new ClaimsPrincipal(identity);
    var ticket = new AuthenticationTicket(principal, Scheme.Name);
    return Task.FromResult(AuthenticateResult.Success(ticket));
  }
}