using System;

using Shortly.Domain.Identity;

namespace Shortly.Domain.Entities;

public class AccessKey : BaseEntity
{
    public string Name { get; set; } = default!;

    public string TokenHash { get; set; } = default!;

    public long AppUserId { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset? ExpiresAt { get; set; }

    public AppUser? AppUserNavigation { get; set; }
}
