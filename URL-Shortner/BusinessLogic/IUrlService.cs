using URL_Shortner.Models;

namespace URL_Shortner.BusinessLogic
{
    public interface IUrlService
    {
        Task<Url?> GetExistingUrl(string originalUrl);

        Task<Url> SaveNewUrl(string originalUrl);

        Task<Url?> GetUrlByShortUrl(string shortUrl);

        void IncrementClickCount(Url url);

        Task<string> GenerateUniqueShortUrl();
    }
}
