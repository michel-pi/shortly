using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Shortly.Domain.Entities;
using Shortly.Domain.Services;
using Shortly.Infrastructure.Data;

namespace Shortly.Infrastructure.Services;

public class ShortLinksService : IShortLinksService
{
    private readonly AppDbContext _db;
    private readonly IShortCodeGenerator _shortCodeGenerator;

    public ShortLinksService(
        AppDbContext db,
        IShortCodeGenerator shortCodeGenerator)
    {
        _db = db;
        _shortCodeGenerator = shortCodeGenerator;
    }

    public async Task<ShortLink> CreateAsync(
        long userId,
        string targetUrl,
        bool isActive,
        DateTimeOffset? expiresAt = null,
        CancellationToken ct = default)
    {
        if (!targetUrl.Contains("://"))
        {
            targetUrl = "https://" + targetUrl;
        }

        var shortLink = new ShortLink
        {
            AppUserId = userId,
            IsActive = isActive,
            TargetUrl = targetUrl,
            ExpiresAt = expiresAt
        };

        _db.ShortLinks.Add(shortLink);

        var created = false;
        do
        {
            try
            {
                shortLink.ShortCode = _shortCodeGenerator.Generate();
                shortLink.Id = 0;

                await _db.SaveChangesAsync(ct);

                created = true;
            }
            catch (DbUpdateException) // UNIQUE short code collision
            {
                _db.Entry(shortLink).State = EntityState.Added;
            }
        } while (!created);

        return shortLink;
    }

    public async Task<ShortLink?> GetAsync(
        long id,
        CancellationToken ct = default)
    {
        return await _db.ShortLinks
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: ct);
    }

    public async Task<ShortLink?> GetByShortCodeAsync(
        string shortCode,
        CancellationToken ct = default)
    {
        return await _db.ShortLinks
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ShortCode == shortCode, cancellationToken: ct);
    }

    public async Task<List<ShortLink>> ListAsync(
        long userId,
        int? skip,
        int? take,
        CancellationToken ct = default)
    {
        var query = _db.ShortLinks
            .AsNoTracking()
            .Where(x => x.AppUserId == userId)
            .OrderByDescending(x => x.CreatedAt);

        if (skip == null && take == null)
        {
            return await query.ToListAsync(cancellationToken: ct);
        }

        IQueryable<ShortLink> pagination = query;
        if (skip != null)
        {
            pagination = pagination.Skip(skip.Value);
        }

        if (take != null)
        {
            pagination = pagination.Take(take.Value);
        }

        return await pagination.ToListAsync(cancellationToken: ct);
    }

    public async Task<ShortLink?> UpdateAsync(
        long id,
        bool? isActive,
        DateTimeOffset? expiresAt,
        CancellationToken ct = default)
    {
        var entity = await _db.ShortLinks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: ct);

        if (entity == null)
        {
            return null;
        }

        if (isActive != null)
        {
            entity.IsActive = isActive.Value;
        }

        if (expiresAt != null)
        {
            entity.ExpiresAt = expiresAt.Value;
        }

        await _db.SaveChangesAsync(ct);

        return entity;
    }

    public async Task<bool> DeleteAsync(
        long id,
        CancellationToken ct = default)
    {
        var entity = await _db.ShortLinks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: ct);

        if (entity == null)
        {
            return false;
        }

        _db.ShortLinks.Remove(entity);

        await _db.SaveChangesAsync(ct);

        return true;
    }
}
