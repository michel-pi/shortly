using System;
using System.Collections.Generic;
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
        string? country);

    Task<ShortLinkEngagement?> GetAsync(long id);

    Task<ShortLinkEngagementSummaryResponse> GetSummaryAsync(
        long userId,
        bool? includeInactive,
        DateTimeOffset? from,
        DateTimeOffset? to);

    Task<List<ShortLinkEngagement>> ListAsync(
        long userId,
        bool? includeInactive,
        DateTimeOffset? from,
        DateTimeOffset? to,
        int? skip,
        int? take,
        long? shortLinkId);
}
