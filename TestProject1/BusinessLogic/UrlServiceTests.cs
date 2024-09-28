using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URL_Shortner.ApplicationDbContext;
using URL_Shortner.BusinessLogic;
using URL_Shortner.Models;

namespace URL_Shortner.Tests.BusinessLogic
{
    public class UrlServiceTests
{
        private UrlContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<UrlContext>()
                .UseInMemoryDatabase(databaseName: "UrlServiceTestDb")
                .Options;

            var context = new UrlContext(options);
            return context;
        }

        [Fact]
        public async Task GetExistingUrl_ShouldReturnUrl_WhenUrlExists()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var urlService = new UrlService(context);
            var existingUrl = new Url { OriginalUrl = "https://test.com", ShortUrl = "short123", ClickCount = 5 };
            context.Urls.Add(existingUrl);
            context.SaveChanges();

            // Act
            var result = await urlService.GetExistingUrl("https://test.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("https://test.com", result.OriginalUrl);
            Assert.Equal("short123", result.ShortUrl);
        }

        [Fact]
        public async Task GetExistingUrl_ShouldReturnNull_WhenUrlDoesNotExist()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var urlService = new UrlService(context);

            // Act
            var result = await urlService.GetExistingUrl("https://nonexistent.com");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SaveNewUrl_ShouldSaveAndReturnNewUrl()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var urlService = new UrlService(context);

            // Act
            var result = await urlService.SaveNewUrl("https://newurl.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("https://newurl.com", result.OriginalUrl);
            Assert.True(context.Urls.Any(u => u.OriginalUrl == "https://newurl.com"));
        }

        [Fact]
        public async Task GetUrlByShortUrl_ShouldReturnUrl_WhenUrlExists()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var urlService = new UrlService(context);
            var existingUrl = new Url { OriginalUrl = "https://test.com", ShortUrl = "short123", ClickCount = 5 };
            context.Urls.Add(existingUrl);
            context.SaveChanges();

            // Act
            var result = await urlService.GetUrlByShortUrl("short123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("short123", result.ShortUrl);
        }

        [Fact]
        public async Task GetUrlByShortUrl_ShouldReturnNull_WhenUrlDoesNotExist()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var urlService = new UrlService(context);

            // Act
            var result = await urlService.GetUrlByShortUrl("nonexistent123");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void IncrementClickCount_ShouldIncrementCount()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var urlService = new UrlService(context);
            var url = new Url { OriginalUrl = "https://test.com", ShortUrl = "short123", ClickCount = 5 };
            context.Urls.Add(url);
            context.SaveChanges();

            // Act
            urlService.IncrementClickCount(url);

            // Assert
            Assert.Equal(6, url.ClickCount);
        }

        [Fact]
        public async Task GenerateUniqueShortUrl_ShouldReturnUniqueShortUrl()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var urlService = new UrlService(context);

            // Act
            var shortUrl = await urlService.GenerateUniqueShortUrl();

            // Assert
            Assert.NotNull(shortUrl);
            Assert.Equal(6, shortUrl.Length);
        }
    }
}
