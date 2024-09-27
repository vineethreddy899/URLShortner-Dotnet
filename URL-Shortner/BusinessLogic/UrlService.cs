using Microsoft.EntityFrameworkCore;
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
            var shortUrl = GenerateUniqueShortUrl();

            var newUrl = new Url
            {
                OriginalUrl = originalUrl,
                ShortUrl = shortUrl.Result
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

        public async Task<string> GenerateUniqueShortUrl()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            string shortUrl;

            // Loop until a unique short URL is found
            do
            {
                shortUrl = new string(Enumerable.Repeat(chars, 6)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            } while (await _context.Urls.AnyAsync(u => u.ShortUrl == BaseUrl + shortUrl));

            return shortUrl;
        }
    }
}
