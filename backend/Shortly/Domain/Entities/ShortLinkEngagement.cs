namespace Shortly.Domain.Entities;

public class ShortLinkEngagement : BaseEntity
{
    public long ShortLinkId { get; set; }

    public string ClientAddressHash { get; set; } = default!;

    public string? UserAgent { get; set; } = default!;

    public string? Referer { get; set; } = default!;

    public string? Country { get; set; } = default!;

    public ShortLink? ShortLinkNavigation { get; set; } 
}
