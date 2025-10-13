using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Shortly.Domain.Entities;

namespace Shortly.Domain.Services;

public interface IAccessKeysService
{
    Task<AccessKey> CreateAsync(
        long userId,
        string name,
        bool isActive,
        DateTimeOffset? expiresAt = null);

    Task<List<AccessKey>> ListAsync(
        long userId,
        int? skip,
        int? take);

    Task<AccessKey?> GetAsync(long id);

    Task<AccessKey?> UpdateAsync(
        long id,
        string? name = null,
        bool? isActive = null,
        DateTimeOffset? expiresAt = null);

    Task<bool> DeleteAsync(long id);
}
