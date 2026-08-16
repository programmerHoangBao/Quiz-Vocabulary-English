using back_end.Models;
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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.GoogleId)
                .IsUnique();

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
            modelBuilder.Entity<VocabularyProgress>()
                .HasIndex(vp => new
                {
                    vp.UserId,
                    vp.VocabularyId
                })
                .IsUnique();
        }
    }
}
