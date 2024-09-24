using Microsoft.AspNetCore.Mvc;
using URL_Shortner.ApplicationDbContext;
using URL_Shortner.Models;

namespace URL_Shortner.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UrlController : ControllerBase
    {
        private readonly UrlContext _context;
        private const string BaseUrl = "https://shortly/";

        public UrlController(UrlContext context)
        {
            _context = context;
        }

        [HttpPost("shortUrl")]
        public IActionResult ShortenUrl([FromBody] string originalUrl)
        {
            // Check if the original URL already exists
            var existingUrl = _context.Urls.FirstOrDefault(u => u.OriginalUrl == originalUrl);
            if (existingUrl != null)
            {
                return Ok(existingUrl.ShortUrl);
            }

            // Generate a short URL
            var shortUrl = GenerateShortUrl();

            // Save the original and short URL to the database
            var url = new Url
            {
                OriginalUrl = originalUrl,
                ShortUrl = BaseUrl + shortUrl
            };
            _context.Urls.Add(url);
            _context.SaveChanges();

            return Ok(url.ShortUrl);
        }

        [HttpGet("{shortUrl}")]
        public IActionResult GetOriginalUrl(string shortUrl)
        {
            var fullShortUrl = BaseUrl + shortUrl;
            var url = _context.Urls.FirstOrDefault(u => u.ShortUrl == fullShortUrl);

            if (url == null || url.OriginalUrl == null)
            {
                return NotFound("URL not found.");
            }

            return Ok(url.OriginalUrl);
        }

        private string GenerateShortUrl()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
