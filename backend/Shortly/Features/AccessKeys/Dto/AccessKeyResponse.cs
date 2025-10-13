using System;

namespace Shortly.Features.AccessKeys.Dto;

public record AccessKeyResponse
{
    public long Id { get; init; }

    public string Name { get; set; } = default!;

    public string Token { get; init; } = default!;

    public bool IsActive { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}
