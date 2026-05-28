using Microsoft.AspNetCore.Diagnostics;

public class GlobalExceptionHandler : IExceptionHandler
{
  public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
  {
    var (status, message) = exception switch
    {
      NotFoundException => (404, exception.Message),
      ConflictException => (409, exception.Message),
      ValidationException => (403, exception.Message),
      _ => (0, "")
    };
    if (status == 0) return false;

    httpContext.Response.StatusCode = status;
    await httpContext.Response.WriteAsJsonAsync(new { status, error = message }, cancellationToken);
    return true;
  }
}