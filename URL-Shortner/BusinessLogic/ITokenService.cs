using URL_Shortner.Models;

namespace URL_Shortner.BusinessLogic
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
