using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PharmacyManagement.API.DTOs;
using PharmacyManagement.API.Models;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return Unauthorized(new { message = "Invalid email or password." });

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);

        return new AuthResponseDto(
            token.Token,
            token.Expiration,
            new UserDto(user.Id, user.FullName, user.Email!, roles.FirstOrDefault() ?? ""));
    }

    [HttpPost("signup")]
    public async Task<ActionResult<AuthResponseDto>> SignUp(SignUpDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            return Conflict(new { message = "A user with this email already exists." });

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(new { message = "Registration failed.", errors = result.Errors.Select(e => e.Description) });

        await _userManager.AddToRoleAsync(user, "Pharmacist");

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);

        return CreatedAtAction(nameof(GetCurrentUser), null, new AuthResponseDto(
            token.Token,
            token.Expiration,
            new UserDto(user.Id, user.FullName, user.Email!, roles.FirstOrDefault() ?? "")));
    }

    [HttpPost("google")]
    public async Task<ActionResult<AuthResponseDto>> GoogleSignIn(GoogleSignInDto dto)
    {
        var googleClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID")
            ?? _configuration["Google:ClientId"];

        if (string.IsNullOrEmpty(googleClientId))
            return StatusCode(500, new { message = "Google sign-in is not configured." });

        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { googleClientId }
            });
        }
        catch (InvalidJwtException)
        {
            return Unauthorized(new { message = "Invalid Google token." });
        }

        var user = await _userManager.FindByEmailAsync(payload.Email);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = payload.Email,
                Email = payload.Email,
                FullName = payload.Name ?? payload.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { message = "Account creation failed.", errors = result.Errors.Select(e => e.Description) });

            await _userManager.AddToRoleAsync(user, "Pharmacist");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);

        return new AuthResponseDto(
            token.Token,
            token.Expiration,
            new UserDto(user.Id, user.FullName, user.Email!, roles.FirstOrDefault() ?? ""));
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return Ok(new { message = "If the email exists, a reset link has been sent." });

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        // TODO: Send this token via email. For now, log it in development.
        _logger.LogInformation("Password reset token for {Email}: {Token}", dto.Email, token);

        return Ok(new { message = "If the email exists, a reset link has been sent." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return BadRequest(new { message = "Invalid reset request." });

        var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
        if (!result.Succeeded)
            return BadRequest(new { message = "Password reset failed.", errors = result.Errors.Select(e => e.Description) });

        return Ok(new { message = "Password has been reset successfully." });
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            return Conflict(new { message = "A user with this email already exists." });

        string[] validRoles = { "Admin", "Pharmacist" };
        if (!validRoles.Contains(dto.Role))
            return BadRequest(new { message = "Invalid role. Must be Admin or Pharmacist." });

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(new { message = "Registration failed.", errors = result.Errors.Select(e => e.Description) });

        await _userManager.AddToRoleAsync(user, dto.Role);

        return CreatedAtAction(nameof(GetCurrentUser), null,
            new UserDto(user.Id, user.FullName, user.Email!, dto.Role));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (user == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        return new UserDto(user.Id, user.FullName, user.Email!, roles.FirstOrDefault() ?? "");
    }

    private (string Token, DateTime Expiration) GenerateJwtToken(ApplicationUser user, IList<string> roles)
    {
        var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
            ?? _configuration["Jwt:Key"]!;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.FullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiryMinutes = int.TryParse(_configuration["Jwt:ExpiryInMinutes"], out var m) ? m : 480;
        var expiration = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiration);
    }
}
