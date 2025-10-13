using System;

namespace Shortly.Domain.Entities;

public class BaseEntity
{
    public long Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? ChangedAt { get; set; }
}
