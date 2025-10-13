using System;

namespace Shortly.Features.ShortLinks.Dto;

public record CreateShortLinkRequest
{
    public string TargetUrl { get; init; } = default!;

    public bool IsActive { get; init; }

    public DateTime? ExpiresAt { get; init; }
}
