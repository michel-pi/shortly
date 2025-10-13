using System;

namespace Shortly.Features.ShortLinks.Dto;

public record ShortLinkResponse
{
    public long Id { get; init; }

    public string TargetUrl { get; init; } = default!;

    public string ShortCode { get; init; } = default!;

    public bool IsActive { get; init; }

    public DateTimeOffset CreatedAt { get; set; }
}
