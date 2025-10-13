using System;
using System.Threading.Tasks;

using Shortly.Domain.Entities;
using Shortly.Domain.Identity;

namespace Shortly.Domain.Services;

public interface IRefreshTokenService
{

    Task<(Guid, DateTimeOffset)> IssueAsync(AppUser user);
    Task<(Guid, DateTimeOffset)> RotateAsync(Guid token);
    Task RevokeAsync(Guid token);
    Task RevokeAllAsync(long userId);
    Task<RefreshToken> GetRefreshTokenAsync(Guid token);
}
