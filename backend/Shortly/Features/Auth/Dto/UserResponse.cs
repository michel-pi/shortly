using System.Collections.Generic;

namespace Shortly.Features.Auth.Dto;

public record UserResponse
{
    public long Id { get; init; }

    public string Email { get; init; } = default!;

    public string Name { get; init; } = default!;

    public List<string> Roles { get; init; } = [];
}
