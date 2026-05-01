using System.Security.Claims;

namespace PharmacyManagement.API.Services;

public interface ITenantService
{
    int? TenantId { get; }
    bool IsSuperAdmin { get; }
}

public class TenantService : ITenantService
{
    public int? TenantId { get; }
    public bool IsSuperAdmin { get; }

    public TenantService(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated == true)
        {
            IsSuperAdmin = user.IsInRole("SuperAdmin");
            var tenantClaim = user.FindFirstValue("TenantId");
            if (int.TryParse(tenantClaim, out var tid))
                TenantId = tid;
        }
    }
}
