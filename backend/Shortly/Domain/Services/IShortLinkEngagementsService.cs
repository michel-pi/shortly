using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Shortly.Domain.Entities;
using Shortly.Features.ShortLinkStats.Dto;

namespace Shortly.Domain.Services;

public interface IShortLinkEngagementsService
{
    Task<ShortLinkEngagement> CreateAsync(
        long shortLinkId,
        string clientIp,
        string? userAgent,
        string? referer,
        string? country,
        CancellationToken ct = default);

    Task<ShortLinkEngagement?> GetAsync(
        long id,
        CancellationToken ct = default);

    Task<ShortLinkEngagementSummaryResponse> GetSummaryAsync(
        long userId,
        bool? includeInactive,
        DateTimeOffset? from,
        DateTimeOffset? to,
        CancellationToken ct = default);

    Task<List<ShortLinkEngagement>> ListAsync(
        long userId,
        bool? includeInactive,
        DateTimeOffset? from,
        DateTimeOffset? to,
        int? skip,
        int? take,
        long? shortLinkId,
        CancellationToken ct = default);
}
