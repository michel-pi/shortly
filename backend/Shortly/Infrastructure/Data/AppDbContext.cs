using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Shortly.Domain.Entities;
using Shortly.Domain.Identity;

namespace Shortly.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, long>
{
    public DbSet<AccessKey> AccessKeys => Set<AccessKey>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<ShortLink> ShortLinks => Set<ShortLink>();

    public DbSet<ShortLinkEngagement> ShortLinkEngagements => Set<ShortLinkEngagement>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AccessKey>(e =>
        {
            e.ToTable("access_keys");
            e.HasKey(x => x.Id);

            e.Property(x => x.AppUserId).IsRequired();
            e.Property(x => x.ChangedAt);
            e.Property(x => x.CreatedAt).IsRequired();
            e.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
            e.Property(x => x.TokenHash).IsRequired().HasMaxLength(64);
            e.Property(x => x.Name).IsRequired().HasMaxLength(128);
            e.Property(x => x.ExpiresAt);

            e.HasIndex(x => x.TokenHash).IsUnique();
            e.HasIndex(x => x.AppUserId);
            e.HasIndex(x => x.IsActive);

            e.HasOne(x => x.AppUserNavigation)
                .WithMany()
                .HasForeignKey(x => x.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<RefreshToken>(e =>
        {
            e.ToTable("refresh_tokens");
            e.HasKey(x => x.Id);

            e.Property(x => x.AppUserId).IsRequired();
            e.Property(x => x.ChangedAt);
            e.Property(x => x.CreatedAt).IsRequired();
            e.Property(x => x.ExpiresAt).IsRequired();
            e.Property(x => x.ReplacedByRefreshTokenId);
            e.Property(x => x.RevokedAt);
            e.Property(x => x.TokenHash).IsRequired().HasMaxLength(64);

            e.HasIndex(x => x.TokenHash).IsUnique();
            e.HasIndex(x => x.AppUserId);
            e.HasIndex(x => x.ExpiresAt);

            e.HasOne(x => x.AppUserNavigation)
                .WithMany()
                .HasForeignKey(x => x.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ShortLink>(e =>
        {
            e.ToTable("short_links");
            e.HasKey(x => x.Id);

            e.Property(x => x.AppUserId).IsRequired();
            e.Property(x => x.ChangedAt);
            e.Property(x => x.CreatedAt).IsRequired();
            e.Property(x => x.TargetUrl).IsRequired().HasMaxLength(1024);
            e.Property(x => x.ShortCode).IsRequired();
            e.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
            e.Property(x => x.ExpiresAt);

            e.HasIndex(x => x.ShortCode).IsUnique();
            e.HasIndex(x => x.AppUserId);

            e.HasOne(x => x.AppUserNavigation)
                .WithMany()
                .HasForeignKey(x => x.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ShortLinkEngagement>(e =>
        {
            e.ToTable("short_link_engagements");
            e.HasKey(x => x.Id);

            e.Property(x => x.ShortLinkId).IsRequired();
            e.Property(x => x.ChangedAt);
            e.Property(x => x.CreatedAt).IsRequired();
            e.Property(x => x.ClientAddressHash).IsRequired().HasMaxLength(64);
            e.Property(x => x.UserAgent).HasMaxLength(1024);
            e.Property(x => x.Referer).HasMaxLength(1024);
            e.Property(x => x.Country).HasMaxLength(128);

            e.HasIndex(x => x.ShortLinkId).IsUnique();
            e.HasIndex(x => x.CreatedAt);

            e.HasOne(x => x.ShortLinkNavigation)
                .WithMany()
                .HasForeignKey(x => x.ShortLinkId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    break;
                case EntityState.Modified:
                    entry.Entity.ChangedAt = now;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
