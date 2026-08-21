using back_end.Models;
using back_end.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace back_end.Data
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Vocabolury> Vocaboluries { get; set; }
        public DbSet<VocabularyProgress> VocabularyProgresses { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries<IAuditable>()
                .Where(e =>
                    e.State == EntityState.Added ||
                    e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.Created = DateTime.UtcNow;
                    entry.Entity.IsDeleted = false;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.LastUpdated = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Create unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.GoogleId)
                .IsUnique();
            modelBuilder.Entity<VocabularyProgress>()
                .HasIndex(vp => new
                {
                    vp.UserId,
                    vp.VocabularyId
                })
                .IsUnique();
            modelBuilder.Entity<RefreshToken>()
                .HasIndex(x => x.TokenHash)
                .IsUnique();

            // Creat index
            modelBuilder.Entity<Folder>()
                .HasIndex(f => f.UserId);
            modelBuilder.Entity<Topic>()
                .HasIndex(t => t.FolderId);
            modelBuilder.Entity<Vocabolury>()
                .HasIndex(v => v.TopicId);
            modelBuilder.Entity<VocabularyProgress>()
                .HasIndex(vp => vp.UserId);
            modelBuilder.Entity<VocabularyProgress>()
                .HasIndex(vp => vp.VocabularyId);
            modelBuilder.Entity<RefreshToken>()
                .HasIndex(x => x.UserId);

            // Create foreignkey
            modelBuilder.Entity<Folder>()
                .HasOne(f => f.User)
                .WithMany(u => u.Folders)
                .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<Topic>()
                .HasOne(t => t.Folder)
                .WithMany(f => f.Topics)
                .HasForeignKey(t => t.FolderId);
            modelBuilder.Entity<Vocabolury>()
                .HasOne(v => v.Topic)
                .WithMany(t => t.Vocaboluries)
                .HasForeignKey(v => v.TopicId);
            modelBuilder.Entity<VocabularyProgress>()
                .HasOne(vp => vp.User)
                .WithMany(u => u.VocabularyProgresses)
                .HasForeignKey(vp => vp.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<VocabularyProgress>()
                .HasOne(vp => vp.Vocabulary)
                .WithMany(v => v.VocabularyProgresses)
                .HasForeignKey(vp => vp.VocabularyId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<RefreshToken>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
