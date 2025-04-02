using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using URL_Shortner.ApplicationDbContext;
using URL_Shortner.Models;

namespace URL_Shortner.BusinessLogic
{
    public class UrlService : IUrlService
    {
        private readonly UrlContext _context;
        private const string BaseUrl = "https://shortly/";
        private const string Base62Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public UrlService(UrlContext context)
        {
            _context = context;
        }

        public async Task<Url?> GetExistingUrl(string originalUrl)
        {
            return await _context.Urls.FirstOrDefaultAsync(u => u.OriginalUrl == originalUrl);
        }

        public async Task<Url> SaveNewUrl(string originalUrl)
        {
            var shortUrl = GenerateShortUrl(originalUrl);

            var newUrl = new Url
            {
                OriginalUrl = originalUrl,
                ShortUrl = shortUrl
            };

            await _context.Urls.AddAsync(newUrl);
            _context.SaveChanges();

            return newUrl;
        }

        public async Task<Url?> GetUrlByShortUrl(string shortUrl)
        {
            return await _context.Urls.FirstOrDefaultAsync(u => u.ShortUrl == shortUrl);
        }

        public void IncrementClickCount(Url url)
        {
            url.ClickCount += 1;
            _context.SaveChanges();
        }

        private static string GenerateShortUrl(string originalUrl)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(originalUrl));
                StringBuilder result = new StringBuilder();

                for (int i = 0; i < 6; i++)  // Take first 6 characters from Base62
                {
                    result.Append(Base62Chars[hashBytes[i] % 62]);
                }

                return BaseUrl + result.ToString();
            }
        }
    }
}
