﻿using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class ProjectDbContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<Articles> Articles { get; set; }
        public DbSet<Follow> Follows { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Follow>().
                        HasKey(uu =>
                        new { uu.FollowerId,
                              uu.FollowedId
                            });
            modelBuilder.Entity<Follow>()
                        .HasOne(u => u.FollowedUser)
                        .WithMany(u => u.Followers)
                        .HasForeignKey(u => u.FollowerId)
                        .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Follow>()
                        .HasOne(u => u.FollowerUser)
                        .WithMany(u => u.Following)
                        .HasForeignKey(u => u.FollowedId)
                        .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Articles>()
                        .HasOne<Users>(s => s.User)
                        .WithMany(g => g.PublishedArticles)
                        .IsRequired()
                        .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Comments>()
                        .HasOne(u => u.User).WithMany()
                        .HasForeignKey(u => u.UserId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-D57N26P;Initial Catalog=FinalProject;Integrated Security=True");
        }
    }
}
