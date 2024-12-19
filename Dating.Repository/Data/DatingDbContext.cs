using Dating.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dating.Repository.Data;

public class DatingDbContext(DbContextOptions<DatingDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }
    public DbSet<UserLike> Likes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserLike>()
            .HasKey(k => new { k.SourceUserId, k.TargetUserId });

        builder.Entity<UserLike>()
            .HasOne(k => k.SourceUser)
            .WithMany(k => k.LikedUsers)
            .HasForeignKey(k => k.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserLike>()
            .HasOne(k => k.TargetUser)
            .WithMany(k => k.LikedByUsers)
            .HasForeignKey(k => k.TargetUserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}