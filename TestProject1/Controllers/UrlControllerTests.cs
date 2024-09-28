using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URL_Shortner.BusinessLogic;
using URL_Shortner.Controllers;
using URL_Shortner.Models;

namespace URL_Shortner.Tests.Controllers
{
    public class UrlControllerTests
{
        private readonly Mock<IUrlService> _mockUrlService;
        private readonly UrlController _urlController;

        public UrlControllerTests()
        {
            _mockUrlService = new Mock<IUrlService>();
            _urlController = new UrlController(_mockUrlService.Object);
        }

        [Fact]
        public async Task ShortenUrl_ShouldReturnExistingShortUrl_WhenUrlExists()
        {
            // Arrange
            var originalUrl = "https://example.com";
            var shortUrl = "short123";
            _mockUrlService.Setup(s => s.GetExistingUrl(originalUrl))
                .ReturnsAsync(new Url { OriginalUrl = originalUrl, ShortUrl = shortUrl });

            // Act
            var result = await _urlController.ShortenUrl(originalUrl);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(shortUrl, okResult.Value);
        }

        [Fact]
        public async Task ShortenUrl_ShouldReturnNewShortUrl_WhenUrlDoesNotExist()
        {
            // Arrange
            var originalUrl = "https://newurl.com";
            var newShortUrl = "short456";
            _mockUrlService.Setup(s => s.GetExistingUrl(originalUrl))
                .ReturnsAsync((Url)null);
            _mockUrlService.Setup(s => s.SaveNewUrl(originalUrl))
                .ReturnsAsync(new Url { OriginalUrl = originalUrl, ShortUrl = newShortUrl });

            // Act
            var result = await _urlController.ShortenUrl(originalUrl);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(newShortUrl, okResult.Value);
        }

        [Fact]
        public async Task GetOriginalUrl_ShouldReturnOriginalUrl_WhenShortUrlExists()
        {
            // Arrange
            var shortUrl = "short123";
            var originalUrl = "https://example.com";
            _mockUrlService.Setup(s => s.GetUrlByShortUrl(shortUrl))
                .ReturnsAsync(new Url { ShortUrl = shortUrl, OriginalUrl = originalUrl });

            // Act
            var result = await _urlController.GetOriginalUrl(shortUrl);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(originalUrl, okResult.Value);
            _mockUrlService.Verify(s => s.IncrementClickCount(It.IsAny<Url>()), Times.Once);
        }

        [Fact]
        public async Task GetOriginalUrl_ShouldReturnNotFound_WhenShortUrlDoesNotExist()
        {
            // Arrange
            var shortUrl = "nonexistent123";
            _mockUrlService.Setup(s => s.GetUrlByShortUrl(shortUrl))
                .ReturnsAsync((Url)null);

            // Act
            var result = await _urlController.GetOriginalUrl(shortUrl);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Short URL not found.", notFoundResult.Value);
            _mockUrlService.Verify(s => s.IncrementClickCount(It.IsAny<Url>()), Times.Never);
        }

        [Fact]
        public async Task GetClickCount_ShouldReturnClickCount_WhenShortUrlExists()
        {
            // Arrange
            var shortUrl = "short123";
            var clickCount = 10;
            _mockUrlService.Setup(s => s.GetUrlByShortUrl(shortUrl))
                .ReturnsAsync(new Url { ShortUrl = shortUrl, ClickCount = clickCount });

            // Act
            var result = await _urlController.GetClickCount(shortUrl);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;

            // Use reflection to dynamically access properties of the anonymous type
            var responseType = response.GetType();
            var shortUrlValue = responseType.GetProperty("ShortUrl").GetValue(response, null);
            var clickCountValue = responseType.GetProperty("ClickCount").GetValue(response, null);

            Assert.Equal(shortUrl, shortUrlValue);
            Assert.Equal(clickCount, clickCountValue);
        }

        [Fact]
        public async Task GetClickCount_ShouldReturnNotFound_WhenShortUrlDoesNotExist()
        {
            // Arrange
            var shortUrl = "nonexistent123";
            _mockUrlService.Setup(s => s.GetUrlByShortUrl(shortUrl))
                .ReturnsAsync((Url)null);

            // Act
            var result = await _urlController.GetClickCount(shortUrl);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Short URL not found.", notFoundResult.Value);
        }
    }
}
