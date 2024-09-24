using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using URL_Shortner.Models;

namespace URL_Shortner.ApplicationDbContext
{
    public class UrlContext : DbContext
    {
        public UrlContext(DbContextOptions<UrlContext> options) : base(options)
        {
        }

        public DbSet<Url> Urls { get; set; }
    }
}
