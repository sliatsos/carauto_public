using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CarAuto.ClaimParser;

public class ClaimParser : IClaimParser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClaimParser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public Guid GetUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Guid.Empty;
        }
        return Guid.Parse(userId);
    }
}
