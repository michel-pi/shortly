using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Shortly.Domain.Entities;
using Shortly.Domain.Services;
using Shortly.Features.ShortLinkStats.Dto;
using Shortly.Infrastructure.Data;
using Shortly.Infrastructure.Utilities;

namespace Shortly.Infrastructure.Services;

// TODO: time series data

public class ShortLinkEngagementsService : IShortLinkEngagementsService
{
    private readonly AppDbContext _db;

    public ShortLinkEngagementsService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ShortLinkEngagement> CreateAsync(
        long shortLinkId,
        string clientIp,
        string? userAgent,
        string? referer,
        string? country,
        CancellationToken ct = default)
    {
        var entity = new ShortLinkEngagement
        {
            ClientAddressHash = HashProvider.Sha256HexString(clientIp),
            Country = country,
            Referer = referer,
            ShortLinkId = shortLinkId,
            UserAgent = userAgent
        };

        _db.ShortLinkEngagements.Add(entity);

        await _db.SaveChangesAsync(ct);

        return entity;
    }

    public async Task<ShortLinkEngagement?> GetAsync(
        long id,
        CancellationToken ct = default)
    {
        return await _db.ShortLinkEngagements
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: ct);
    }

    public async Task<ShortLinkEngagementSummaryResponse> GetSummaryAsync(
        long userId,
        bool? includeInactive,
        DateTimeOffset? from,
        DateTimeOffset? to,
        CancellationToken ct = default)
    {
        (from, to) = NormalzeRange(from, to);
        var incInactive = includeInactive == true;

        var query = GetQueryableRange(userId, includeInactive, from, to);

        var totalClicks = await query.LongCountAsync(cancellationToken: ct);

        var totalClients = await query
            .Select(x => x.ClientAddressHash)
            .Distinct()
            .LongCountAsync(cancellationToken: ct);

        var countries = await query
            .Select(x => new { Key = (x.Country == null || x.Country == "") ? "?" : x.Country! })
            .GroupBy(x => x.Key)
            .Select(g => new { g.Key, Count = g.LongCount() })
            .OrderByDescending(x => x.Count)
            .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken: ct);

        var referers = await query
            .Select(x => new { Key = (x.Referer == null || x.Referer == "") ? "?" : x.Referer! })
            .GroupBy(x => x.Key)
            .Select(g => new { g.Key, Count = g.LongCount() })
            .OrderByDescending(x => x.Count)
            .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken: ct);

        return new ShortLinkEngagementSummaryResponse
        {
            TotalClicks = totalClicks,
            TotalClients = totalClients,
            Countries = countries,
            Referers = referers,
            From = from,
            To = to
        };
    }

    public async Task<List<ShortLinkEngagement>> ListAsync(
        long userId,
        bool? includeInactive,
        DateTimeOffset? from,
        DateTimeOffset? to,
        int? skip = null,
        int? take = null,
        long? shortLinkId = null,
        CancellationToken ct = default)
    {
        (from, to) = NormalzeRange(from, to);

        var query = GetQueryableRange(userId, includeInactive, from, to);

        if (shortLinkId != null)
        {
            query = query.Where(x => x.ShortLinkId == shortLinkId);
        }

        if (skip != null)
        {
            query = query.Skip(skip.Value);
        }

        if (take != null)
        {
            query = query.Take(take.Value);
        }

        return await query.ToListAsync(cancellationToken: ct);
    }

    private IQueryable<ShortLinkEngagement> GetQueryableRange(
        long userId,
        bool? includeInactive,
        DateTimeOffset? from,
        DateTimeOffset? to)
    {
        return _db.ShortLinkEngagements
            .AsNoTracking()
            .Join(
                _db.ShortLinks.AsNoTracking().Where(l => l.AppUserId == userId && (includeInactive == true || l.IsActive)),
                e => e.ShortLinkId,
                l => l.Id,
                (e, l) => e
            )
            .Where(x => x.CreatedAt >= from && x.CreatedAt <= to);
    }

    private static (DateTimeOffset from, DateTimeOffset to) NormalzeRange(DateTimeOffset? fromInput, DateTimeOffset? toInput)
    {
        return (fromInput ?? DateTimeOffset.UnixEpoch, toInput ?? DateTimeOffset.UtcNow);
    }
}
