using System.ComponentModel.DataAnnotations;

namespace Shortly.Features.Auth.Dto;

public record LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; init; } = default!;

    [Required, MinLength(1)]
    public string Password { get; init; } = default!;
}
