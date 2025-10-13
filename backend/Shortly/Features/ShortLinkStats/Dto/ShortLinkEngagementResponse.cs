namespace Shortly.Features.ShortLinkStats.Dto;

public record ShortLinkEngagementResponse
{
    public long Id { get; set; }

    public long ShortLinkId { get; set; }

    public string? UserAgent { get; init; }

    public string? Referer { get; init; }

    public string? Country { get; init; }
}
