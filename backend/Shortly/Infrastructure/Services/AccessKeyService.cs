using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Shortly.Domain.Entities;
using Shortly.Domain.Services;
using Shortly.Infrastructure.Data;
using Shortly.Infrastructure.Utilities;

namespace Shortly.Infrastructure.Services;

public class AccessKeyService : IAccessKeysService
{
    private readonly AppDbContext _db;

    public AccessKeyService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AccessKey> CreateAsync(
        long userId,
        string name,
        bool isActive,
        DateTimeOffset? expiresAt = null,
        CancellationToken ct = default)
    {
        var tokenHash = HashProvider.Sha256HexString(Guid.NewGuid().ToString());

        var entity = new AccessKey
        {
            AppUserId = userId,
            ExpiresAt = expiresAt,
            IsActive = isActive,
            Name = name,
            TokenHash = tokenHash
        };

        _db.AccessKeys.Add(entity);

        await _db.SaveChangesAsync(ct);

        return entity;
    }

    public async Task<List<AccessKey>> ListAsync(
        long userId,
        int? skip,
        int? take,
        CancellationToken ct = default)
    {
        var query = _db.AccessKeys
            .AsNoTracking()
            .Where(x => x.AppUserId == userId)
            .OrderByDescending(x => x.Name);

        if (skip == null && take == null)
        {
            return await query.ToListAsync(cancellationToken: ct);
        }

        IQueryable<AccessKey> pagination = query;
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

    public async Task<AccessKey?> GetAsync(
        long id,
        CancellationToken ct = default)
    {
        return await _db.AccessKeys.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: ct);
    }

    public async Task<AccessKey?> UpdateAsync(
        long id,
        string? name = null,
        bool? isActive = null,
        DateTimeOffset? expiresAt = null,
        CancellationToken ct = default)
    {
        var entity = await _db.AccessKeys.FirstOrDefaultAsync(x =>x.Id == id, cancellationToken: ct);

        if (entity == null)
        {
            return null;
        }

        if (name != null)
        {
            entity.Name = name;
        }

        if (isActive != null)
        {
            entity.IsActive = isActive.Value;
        }

        if (expiresAt != null)
        {
            entity.ExpiresAt = expiresAt;
        }

        await _db.SaveChangesAsync(ct);

        return entity;
    }

    public async Task<bool> DeleteAsync(
        long id,
        CancellationToken ct = default)
    {
        var entity = await _db.AccessKeys.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: ct);

        if (entity == null)
        {
            return false;
        }

        _db.AccessKeys.Remove(entity);

        await _db.SaveChangesAsync(ct);

        return true;
    }
}
