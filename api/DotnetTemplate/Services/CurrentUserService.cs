using DotnetTemplate.Exceptions;
using System.Security.Claims;

namespace DotnetTemplate.Services;

public sealed class CurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetUserId()
    {
        var value = _httpContextAccessor
            .HttpContext?
            .User
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value;

        if (string.IsNullOrWhiteSpace(value))
            throw new ApiException(
                StatusCodes.Status401Unauthorized,
                "Usuário não logado.");

        return Guid.Parse(value);
    }

    public string GetEmail()
    {
        return _httpContextAccessor
            .HttpContext?
            .User
            .FindFirst(ClaimTypes.Email)?
            .Value
            ?? throw new ApiException(
                StatusCodes.Status401Unauthorized,
                "Usuário não logado.");
    }
}
