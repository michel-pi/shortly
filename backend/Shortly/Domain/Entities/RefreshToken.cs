using System;

using Shortly.Domain.Identity;

namespace Shortly.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public long AppUserId { get; set; }

    public string TokenHash { get; set; } = default!;

    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset? RevokedAt { get; set; }

    public long? ReplacedByRefreshTokenId { get; set; }

    public AppUser? AppUserNavigation { get; set; }
}
