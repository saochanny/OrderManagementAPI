using System.IdentityModel.Tokens.Jwt;
using OrderManagementAPI.Exceptions;
using OrderManagementAPI.Utilizes;

namespace OrderManagementAPI.Security;

using System.Security.Claims;

public class CurrentUserService (IHttpContextAccessor contextAccessor): ICurrentUserService
{

    private ClaimsPrincipal? User => contextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public int UserId =>
        int.TryParse(User?.FindFirst(JwtTokenUtil.UserIdClaim)?.Value, out var id) ? id : throw new AppException("Can't extract user id");

    public string Username => User?.Identity?.Name ?? string.Empty;

    public string Email => User?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

    public IEnumerable<string> Roles =>
        User?.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? [];
}
