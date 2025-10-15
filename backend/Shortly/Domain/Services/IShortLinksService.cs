using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Shortly.Domain.Entities;

namespace Shortly.Domain.Services;

public interface IShortLinksService
{
    Task<ShortLink> CreateAsync(
        long userId,
        string targetUrl,
        bool isActive,
        DateTimeOffset? expiresAt,
        CancellationToken ct = default);

    Task<ShortLink?> GetAsync(
        long id,
        CancellationToken ct = default);

    Task<ShortLink?> GetByShortCodeAsync(
        string shortCode,
        CancellationToken ct = default);

    Task<List<ShortLink>> ListAsync(
        long userId,
        int? skip,
        int? take,
        CancellationToken ct = default);

    Task<ShortLink?> UpdateAsync(
        long id,
        bool? isActive,
        DateTimeOffset? expiresAt,
        CancellationToken ct = default);

    Task<bool> DeleteAsync(
        long id,
        CancellationToken ct = default);
}
