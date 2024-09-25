using Microsoft.EntityFrameworkCore;
using URL_Shortner.Models;

namespace URL_Shortner.ApplicationDbContext
{
    public class UrlContext : DbContext
    {
        public UrlContext(DbContextOptions<UrlContext> options) : base(options)
        {
        }

        public DbSet<Url> Urls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Enforce uniqueness of ShortUrl
            modelBuilder.Entity<Url>()
                .HasIndex(u => u.ShortUrl)
                .IsUnique();
        }
    }
}
