using System.ComponentModel.DataAnnotations;

namespace Verd.Api.DTOs.Auth;

public record RegisterDto(
    [Required] string DisplayName,
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password,
    string Location = "San Francisco, CA"
);
