using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyManagement.API.Data;
using PharmacyManagement.API.DTOs;
using PharmacyManagement.API.Models;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin")]
public class TenantsController : ControllerBase
{
    private readonly PharmacyDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public TenantsController(PharmacyDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<List<TenantDto>>> GetAll()
    {
        return await _db.Tenants
            .OrderBy(t => t.Name)
            .Select(t => new TenantDto(t.Id, t.Name, t.Slug, t.IsActive, t.CreatedAt))
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TenantDto>> Get(int id)
    {
        var tenant = await _db.Tenants.FindAsync(id);
        if (tenant == null) return NotFound();
        return new TenantDto(tenant.Id, tenant.Name, tenant.Slug, tenant.IsActive, tenant.CreatedAt);
    }

    [HttpPost]
    public async Task<ActionResult<TenantDto>> Create(CreateTenantDto dto)
    {
        var slug = GenerateSlug(dto.Name);
        if (await _db.Tenants.AnyAsync(t => t.Slug == slug))
            return Conflict(new { message = "A pharmacy with a similar name already exists." });

        var existingUser = await _userManager.FindByEmailAsync(dto.AdminEmail);
        if (existingUser != null)
            return Conflict(new { message = "A user with the admin email already exists." });

        var tenant = new Tenant { Name = dto.Name, Slug = slug };
        _db.Tenants.Add(tenant);
        await _db.SaveChangesAsync();

        var adminUser = new ApplicationUser
        {
            UserName = dto.AdminEmail,
            Email = dto.AdminEmail,
            FullName = dto.AdminFullName,
            EmailConfirmed = true,
            TenantId = tenant.Id
        };

        var result = await _userManager.CreateAsync(adminUser, dto.AdminPassword);
        if (!result.Succeeded)
        {
            _db.Tenants.Remove(tenant);
            await _db.SaveChangesAsync();
            return BadRequest(new { message = "Failed to create admin user.", errors = result.Errors.Select(e => e.Description) });
        }

        await _userManager.AddToRoleAsync(adminUser, "Admin");

        return CreatedAtAction(nameof(Get), new { id = tenant.Id },
            new TenantDto(tenant.Id, tenant.Name, tenant.Slug, tenant.IsActive, tenant.CreatedAt));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TenantDto>> Update(int id, UpdateTenantDto dto)
    {
        var tenant = await _db.Tenants.FindAsync(id);
        if (tenant == null) return NotFound();

        tenant.Name = dto.Name;
        tenant.IsActive = dto.IsActive;
        await _db.SaveChangesAsync();

        return new TenantDto(tenant.Id, tenant.Name, tenant.Slug, tenant.IsActive, tenant.CreatedAt);
    }

    private static string GenerateSlug(string name)
    {
        var slug = name.ToLowerInvariant().Trim();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        return slug.Trim('-');
    }
}
