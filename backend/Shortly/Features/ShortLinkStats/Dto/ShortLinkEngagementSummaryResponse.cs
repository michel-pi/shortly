using System;
using System.Collections.Generic;

namespace Shortly.Features.ShortLinkStats.Dto;

public record ShortLinkEngagementSummaryResponse
{
    public long TotalClicks { get; init; }

    public long TotalClients { get; init; }

    public Dictionary<string, long> Countries { get; init; } = [];

    public Dictionary<string, long> Referers { get; init; } = [];

    public DateTimeOffset? From { get; init; }

    public DateTimeOffset? To { get; init; }
}
