using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URL_Shortner.BusinessLogic;

namespace URL_Shortner.Controllers
{
    /// <summary>
    /// Controller for Urls
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class UrlController : ControllerBase
    {
        private readonly IUrlService _urlService;
        private const string BaseUrl = "https://shortly/";

        public UrlController(IUrlService urlService)
        {
            _urlService = urlService;
        }


        /// <summary>
        /// Creates Short Url from Original Url
        /// </summary>
        /// <param name="originalUrl"></param>
        /// <returns>Short Url</returns>
        [HttpPost("createUrl")]
        [Authorize]
        public async Task<IActionResult> ShortenUrl([FromBody] string originalUrl)
        {
            // Check if URL already exists
            var existingUrl = await _urlService.GetExistingUrl(originalUrl);
            if (existingUrl != null)
            {
                return Ok(existingUrl.ShortUrl);
            }

            // Generate and save a new short URL
            var newUrl = await _urlService.SaveNewUrl(originalUrl);

            return Ok(newUrl.ShortUrl);
        }

        /// <summary>
        /// Gets Original Url from Short Url
        /// </summary>
        /// <param name="shortUrl"></param>
        /// <returns>Original Url</returns>
        [HttpGet("{shortUrl}")]
        [Authorize]
        public async Task<IActionResult> GetOriginalUrl(string shortUrl)
        {
            var url = await _urlService.GetUrlByShortUrl(shortUrl);

            if (url == null || url.OriginalUrl == null)
            {
                return NotFound("Short URL not found.");
            }

            // Increment click count
            _urlService.IncrementClickCount(url);

            return Ok(url.OriginalUrl);
        }

        /// <summary>
        /// Gets Count for Url clicks
        /// </summary>
        /// <param name="shortUrl"></param>
        /// <returns>Count details</returns>
        [HttpGet("{shortUrl}/clicks")]
        [Authorize]
        public async Task<IActionResult> GetClickCount(string shortUrl)
        {
            var url = await _urlService.GetUrlByShortUrl(shortUrl);

            if (url == null)
            {
                return NotFound("Short URL not found.");
            }

            return Ok(new { ShortUrl = url.ShortUrl, ClickCount = url.ClickCount });
        }
    }
}
