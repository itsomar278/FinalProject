using DataAcess.Entites;
using Microsoft.EntityFrameworkCore;

namespace DataAcess
{
    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options)
        {
        }
        public DbSet<Users> Users { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<Articles> Articles { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<RefreshTokens> RefreshTokens { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Favorite>()
                .HasKey(ua => new { ua.ArticleId, ua.UserId });
            modelBuilder.Entity<Favorite>()
                        .HasOne(f => f.User)
                        .WithMany(u => u.FavoriteArticles)
                        .HasForeignKey(f => f.UserId)
                        .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Favorite>()
                       .HasOne(f => f.Article)
                       .WithMany(a => a.FavouredBy)
                       .HasForeignKey(f => f.ArticleId)
                       .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Follow>().
                        HasKey(uu =>
                        new
                        {
                            uu.FollowerId,
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
                        .HasOne(s => s.User)
                        .WithMany(g => g.PublishedArticles)
                        .IsRequired()
                        .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Comments>()
                        .HasOne(u => u.User).WithMany()
                        .HasForeignKey(u => u.UserId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
            modelBuilder.Entity<Users>()
                        .HasOne(u => u.RefreshToken)
                        .WithOne(t => t.User)
                        .HasForeignKey<RefreshTokens>(t => t.UserId);
        }
    }
}
