using System;
using System.Threading;
using System.Threading.Tasks;

using Shortly.Domain.Entities;
using Shortly.Domain.Identity;

namespace Shortly.Domain.Services;

public interface IRefreshTokenService
{

    Task<(Guid, DateTimeOffset)> IssueAsync(
        AppUser user,
        CancellationToken ct = default);

    Task<(Guid, DateTimeOffset)> RotateAsync(
        Guid token,
        CancellationToken ct = default);
    
    Task RevokeAsync(
        Guid token,
        CancellationToken ct = default);

    Task RevokeAllAsync(
        long userId,
        CancellationToken ct = default);
    
    Task<RefreshToken> GetRefreshTokenAsync(
        Guid token,
        CancellationToken ct = default);
}
