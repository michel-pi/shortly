using System;

namespace Shortly.Features.AccessKeys.Dto;

public record UpdateAccessKeyRequest
{
    public string? Name { get; init; }

    public bool? IsActive { get; init; }

    public DateTimeOffset? ExpiresAt { get; init; }
}
