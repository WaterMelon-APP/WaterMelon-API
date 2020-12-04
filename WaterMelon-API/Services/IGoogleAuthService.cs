using System.Threading.Tasks;
using WaterMelon_API.Models;

namespace WaterMelon_API.Services
{
    public interface IGoogleAuthService
    {
        Task<GoogleUserInfoResult> ValidateAccessTokenAsync(string accessToken);
    }
}