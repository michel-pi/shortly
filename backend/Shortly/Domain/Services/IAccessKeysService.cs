using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Shortly.Domain.Entities;

namespace Shortly.Domain.Services;

public interface IAccessKeysService
{
    Task<AccessKey> CreateAsync(
        long userId,
        string name,
        bool isActive,
        DateTimeOffset? expiresAt = null,
        CancellationToken ct = default);

    Task<List<AccessKey>> ListAsync(
        long userId,
        int? skip,
        int? take,
        CancellationToken ct = default);

    Task<AccessKey?> GetAsync(
        long id,
        CancellationToken ct = default);

    Task<AccessKey?> UpdateAsync(
        long id,
        string? name = null,
        bool? isActive = null,
        DateTimeOffset? expiresAt = null,
        CancellationToken ct = default);

    Task<bool> DeleteAsync(
        long id,
        CancellationToken ct = default);
}
