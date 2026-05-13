using Microsoft.EntityFrameworkCore;
using Mocny_RasberyPi_Images_Listener.Models;

namespace Mocny_RasberyPi_Images_Listener.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Screen> Screens { get; set; }
        public DbSet<ScreenGroup> ScreenGroups { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<CollectionItem> CollectionItems { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Screen
            modelBuilder.Entity<Screen>()
                .HasIndex(s => s.UniqueIdentifier)
                .IsUnique();

            modelBuilder.Entity<Screen>()
                .HasOne(s => s.Group)
                .WithMany(sg => sg.Screens)
                .HasForeignKey(s => s.GroupId)
                .OnDelete(DeleteBehavior.SetNull);

            // Collection & Items
            modelBuilder.Entity<CollectionItem>()
                .HasOne(ci => ci.Collection)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CollectionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CollectionItem>()
                .HasOne(ci => ci.Image)
                .WithMany()
                .HasForeignKey(ci => ci.ImageId)
                .OnDelete(DeleteBehavior.Restrict);

            // Schedule
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Screen)
                .WithMany(sc => sc.Schedules)
                .HasForeignKey(s => s.ScreenId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Image)
                .WithMany()
                .HasForeignKey(s => s.ImageId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Collection)
                .WithMany()
                .HasForeignKey(s => s.CollectionId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // AuditLog
            modelBuilder.Entity<AuditLog>()
                .HasOne(al => al.User)
                .WithMany()
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
        }
    }
}