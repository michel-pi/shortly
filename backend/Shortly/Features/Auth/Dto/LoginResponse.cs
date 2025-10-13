using System;

namespace Shortly.Features.Auth.Dto;

public record LoginResponse
{
    public string AccessToken { get; init; } = default!;

    public Guid RefreshToken { get; init; }
}
