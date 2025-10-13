using System;

namespace Shortly.Features.ShortLinks.Dto;

public record UpdateShortLinkRequest
{
    public bool? IsActive { get; init; }

    public DateTimeOffset? ExpiresAt { get; init; }
}
