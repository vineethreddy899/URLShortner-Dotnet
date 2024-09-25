using URL_Shortner.Models;

namespace URL_Shortner.BusinessLogic
{
    public interface IUrlService
    {
        Url? GetExistingUrl(string originalUrl);

        Url SaveNewUrl(string originalUrl);

        Url? GetUrlByShortUrl(string shortUrl);

        void IncrementClickCount(Url url);

        string GenerateUniqueShortUrl();
    }
}
