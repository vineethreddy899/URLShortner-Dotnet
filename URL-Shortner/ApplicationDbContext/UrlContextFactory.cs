using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace URL_Shortner.ApplicationDbContext
{
    public class UrlContextFactory : IDesignTimeDbContextFactory<UrlContext>
    {
        public UrlContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UrlContext>();
            optionsBuilder.UseSqlServer("Server=IN-PG02P5S0;Database=Urls;Integrated Security=True;TrustServerCertificate=True");

            return new UrlContext(optionsBuilder.Options);
        }
    }
}
