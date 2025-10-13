using System;

namespace Shortly.Features.AccessKeys.Dto;

public record CreateAccessKeyRequest
{
    public string Name { get; init; } = default!;

    public bool IsActive { get; init; }

    public DateTimeOffset? ExpiresAt { get; init; }
}
