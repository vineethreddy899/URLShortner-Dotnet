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

        public Url? GetExistingUrl(string originalUrl)
        {
            return _context.Urls.FirstOrDefault(u => u.OriginalUrl == originalUrl);
        }

        public Url SaveNewUrl(string originalUrl)
        {
            var shortUrl = GenerateUniqueShortUrl();

            var newUrl = new Url
            {
                OriginalUrl = originalUrl,
                ShortUrl = shortUrl
            };

            _context.Urls.Add(newUrl);
            _context.SaveChanges();

            return newUrl;
        }

        public Url? GetUrlByShortUrl(string shortUrl)
        {
            return _context.Urls.FirstOrDefault(u => u.ShortUrl == shortUrl);
        }

        public void IncrementClickCount(Url url)
        {
            url.ClickCount += 1;
            _context.SaveChanges();
        }

        public string GenerateUniqueShortUrl()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            string shortUrl;

            // Loop until a unique short URL is found
            do
            {
                shortUrl = new string(Enumerable.Repeat(chars, 6)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            } while (_context.Urls.Any(u => u.ShortUrl == BaseUrl + shortUrl));

            return shortUrl;
        }
    }
}
