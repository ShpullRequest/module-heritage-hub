using Microsoft.EntityFrameworkCore;
using ModuleHeritageHub.Domain.Model;

namespace ModuleHeritageHub.Infrastructure.DB
{
    public class DBContext(DbContextOptions<DBContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Image> Images { get; set; } = null!;
        public DbSet<Page> Pages { get; set; } = null!;
        public DbSet<PageVersion> PageVersions { get; set; } = null!;
        public DbSet<PageVersionImage> PageVersionImages { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

             modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();
            
            modelBuilder.Entity<User>()
                .HasOne(u => u.Image)
                .WithMany()
                .HasForeignKey(u => u.ImageId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Page>()
                .HasOne(p => p.CurrentVersion)
                .WithMany()
                .HasForeignKey(p => p.CurrentVersionId);
            
            modelBuilder.Entity<Page>()
                .HasMany(p => p.PageVersions)
                .WithOne(pv => pv.Page)
                .HasForeignKey(pv => pv.PageId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<PageVersion>()
                .HasOne(pv => pv.Owner)
                .WithMany(u => u.PageVersions)
                .HasForeignKey(pv => pv.OwnerId);

            modelBuilder.Entity<PageVersionImage>()
                .HasKey(pvi => new { pvi.VersionId, pvi.ImageId });

            modelBuilder.Entity<PageVersionImage>()
                .HasOne(pvi => pvi.PageVersion)
                .WithMany(pv => pv.Images)
                .HasForeignKey(pvi => pvi.VersionId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<PageVersionImage>()
                .HasOne(pvi => pvi.Image)
                .WithMany()
                .HasForeignKey(pvi => pvi.ImageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}