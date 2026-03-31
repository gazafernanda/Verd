using System.ComponentModel.DataAnnotations;

namespace Verd.Api.DTOs.Auth;

public record LoginDto(
    [Required, EmailAddress] string Email,
    [Required] string Password
);
