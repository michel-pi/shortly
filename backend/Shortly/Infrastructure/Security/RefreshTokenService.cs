using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Shortly.Configuration.Options;
using Shortly.Domain.Entities;
using Shortly.Domain.Identity;
using Shortly.Domain.Services;
using Shortly.Infrastructure.Data;
using Shortly.Infrastructure.Utilities;

namespace Shortly.Infrastructure.Security;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly AppDbContext _db;
    private readonly int _refreshTokenDays;

    public RefreshTokenService(
        AppDbContext db,
        IOptions<JwtOptions> jwtOptions)
    {
        _db = db;
        _refreshTokenDays = jwtOptions.Value.RefreshTokenDays;
    }

    public async Task<(Guid, DateTimeOffset)> IssueAsync(AppUser user, CancellationToken ct = default)
    {
        var now = DateTimeOffset.UtcNow;
        var token = Guid.NewGuid();
        var tokenHash = HashProvider.Sha256HexString(token.ToString());

        var rt = new RefreshToken
        {
            AppUserId = user.Id,
            ExpiresAt = now.AddDays(_refreshTokenDays),
            ReplacedByRefreshTokenId = null,
            RevokedAt = null,
            TokenHash = tokenHash
        };

        _db.RefreshTokens.Add(rt);

        await _db.SaveChangesAsync(ct);

        return (token, rt.ExpiresAt);
    }

    public async Task<(Guid, DateTimeOffset)> RotateAsync(Guid token, CancellationToken ct = default)
    {
        var now = DateTimeOffset.UtcNow;
        var tokenHash = HashProvider.Sha256HexString(token.ToString());

        var currentRt = await _db.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken: ct);

        if (currentRt == null || currentRt.RevokedAt != null || currentRt.ExpiresAt <= now)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        // reused
        if (currentRt.ReplacedByRefreshTokenId != null)
        {
            await RevokeAllAsync(currentRt.AppUserId, ct);

            throw new UnauthorizedAccessException("Refresh token reused.");
        }

        var nextToken = Guid.NewGuid();
        var nextTokenHash = HashProvider.Sha256HexString(nextToken.ToString());

        var nextRt = new RefreshToken
        {
            AppUserId = currentRt.AppUserId,
            ExpiresAt = now.AddDays(_refreshTokenDays),
            ReplacedByRefreshTokenId = null,
            RevokedAt = null,
            TokenHash = nextTokenHash
        };

        _db.RefreshTokens.Add(nextRt);
        currentRt.RevokedAt = now;

        await _db.SaveChangesAsync(ct);

        currentRt.ReplacedByRefreshTokenId = nextRt.Id;

        await _db.SaveChangesAsync(ct);

        return (nextToken, nextRt.ExpiresAt);
    }

    public async Task RevokeAsync(Guid token, CancellationToken ct = default)
    {
        var tokenHash = HashProvider.Sha256HexString(token.ToString());

        var currentRt = await _db.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken: ct);

        if (currentRt == null)
        {
            return;
        }

        currentRt.RevokedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
    }

    public async Task RevokeAllAsync(long userId, CancellationToken ct = default)
    {
        var now = DateTimeOffset.UtcNow;

        await _db.RefreshTokens
            .Where(x => x.AppUserId == userId && x.RevokedAt == null)
            .ExecuteUpdateAsync(x => x.SetProperty(rt => rt.RevokedAt, now), cancellationToken: ct);
    }

    public async Task<RefreshToken> GetRefreshTokenAsync(Guid token, CancellationToken ct = default)
    {
        var tokenHash = HashProvider.Sha256HexString(token.ToString());

        var rt = await _db.RefreshTokens
            .AsNoTracking()
            .Include(x => x.AppUserNavigation)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken: ct);

        if (rt == null)
        {
            throw new UnauthorizedAccessException("Refresh token not found.");
        }

        return rt;
    }
}
