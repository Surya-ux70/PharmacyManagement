using Microsoft.AspNetCore.Identity;

namespace PharmacyManagement.API.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? TenantId { get; set; }
    public Tenant? Tenant { get; set; }
}
