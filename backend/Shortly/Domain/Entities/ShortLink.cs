using System;

using Shortly.Domain.Identity;

namespace Shortly.Domain.Entities;

public class ShortLink : BaseEntity
{
    public long AppUserId { get; set; }

    public string TargetUrl { get; set; } = default!;

    public string ShortCode { get; set; } = default!;

    public bool IsActive { get; set; }

    public DateTimeOffset? ExpiresAt { get; set; }

    public AppUser? AppUserNavigation { get; set; }
}
