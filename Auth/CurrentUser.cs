using System.Security.Claims;

public interface ICurrentUser
{
  int UserId {get;}
  Role Role {get;}
}

public class CurrentUser : ICurrentUser
{
  private readonly IHttpContextAccessor _accessor;
  public CurrentUser(IHttpContextAccessor accessor) => _accessor = accessor;
  public int UserId => int.Parse(_accessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

  public Role Role => Enum.Parse<Role>(_accessor.HttpContext!.User.FindFirstValue(ClaimTypes.Role)!);
}