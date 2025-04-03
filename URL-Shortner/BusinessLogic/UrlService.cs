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
            // Add a salt (timestamp ensures uniqueness even for identical URLs)
            string saltedUrl = originalUrl + DateTime.UtcNow.Ticks;

            // Generate SHA-256 hash
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedUrl));

                // Convert hash to Base62 and take the first 8 characters
                return BaseUrl + Base62Encode(hashBytes).Substring(0, 8);
            }
        }

        private static string Base62Encode(byte[] bytes)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder result = new StringBuilder();
            ulong value = BitConverter.ToUInt64(bytes, 0); // Convert first 8 bytes to a number

            while (value > 0)
            {
                result.Insert(0, chars[(int)(value % 62)]);
                value /= 62;
            }

            return result.ToString();
        }
    }
}
