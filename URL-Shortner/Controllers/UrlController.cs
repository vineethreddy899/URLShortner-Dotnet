using Microsoft.AspNetCore.Mvc;
using URL_Shortner.BusinessLogic;

namespace URL_Shortner.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UrlController : ControllerBase
    {
        private readonly UrlService _urlService;
        private const string BaseUrl = "https://shortly/";

        public UrlController(UrlService urlService)
        {
            _urlService = urlService;
        }

        [HttpPost("shorten")]
        public IActionResult ShortenUrl([FromBody] string originalUrl)
        {
            // Check if URL already exists
            var existingUrl = _urlService.GetExistingUrl(originalUrl);
            if (existingUrl != null)
            {
                return Ok(existingUrl.ShortUrl);
            }

            // Generate and save a new short URL
            var newUrl = _urlService.SaveNewUrl(originalUrl);

            return Ok(newUrl.ShortUrl);
        }

        [HttpGet("{shortUrl}")]
        public IActionResult GetOriginalUrl(string shortUrl)
        {
            var url = _urlService.GetUrlByShortUrl(shortUrl);

            if (url == null || url.OriginalUrl == null)
            {
                return NotFound("Short URL not found.");
            }

            // Increment click count
            _urlService.IncrementClickCount(url);

            return Ok(url.OriginalUrl);
        }

        [HttpGet("{shortUrl}/clicks")]
        public IActionResult GetClickCount(string shortUrl)
        {
            var url = _urlService.GetUrlByShortUrl(shortUrl);

            if (url == null)
            {
                return NotFound("Short URL not found.");
            }

            return Ok(new { ShortUrl = url.ShortUrl, ClickCount = url.ClickCount });
        }
    }
}
