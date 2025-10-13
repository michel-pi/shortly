using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Shortly.Domain.Entities;

namespace Shortly.Domain.Services;

public interface IShortLinksService
{
    Task<ShortLink> CreateAsync(
        long userId,
        string targetUrl,
        bool isActive,
        DateTimeOffset? expiresAt);

    Task<ShortLink?> GetAsync(long id);

    Task<ShortLink?> GetByShortCodeAsync(string shortCode);

    Task<List<ShortLink>> ListAsync(
        long userId,
        int? skip,
        int? take);

    Task<ShortLink?> UpdateAsync(
        long id,
        bool? isActive,
        DateTimeOffset? expiresAt);

    Task<bool> DeleteAsync(long id);
}
