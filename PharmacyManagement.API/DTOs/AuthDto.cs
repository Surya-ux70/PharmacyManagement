using System.ComponentModel.DataAnnotations;

namespace PharmacyManagement.API.DTOs;

public record LoginDto(
    [Required] string Email,
    [Required] string Password);

public record RegisterDto(
    [Required] string FullName,
    [Required][EmailAddress] string Email,
    [Required][MinLength(6)] string Password,
    [Required] string Role);

public record AuthResponseDto(
    string Token,
    DateTime Expiration,
    UserDto User);

public record UserDto(
    string Id,
    string FullName,
    string Email,
    string Role);
