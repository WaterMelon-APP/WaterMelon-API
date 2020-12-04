using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WaterMelon_API.Models;

namespace WaterMelon_API.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private const string GoogleUserInfoUrl = "https://oauth2.googleapis.com/tokeninfo?id_token={0}";
        private readonly IHttpClientFactory _httpClientFactory;

        public GoogleAuthService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<GoogleUserInfoResult> ValidateAccessTokenAsync(string accessToken)
        {
            var formattedUrl = string.Format(GoogleUserInfoUrl, accessToken);
            var result = await _httpClientFactory.CreateClient().GetAsync(formattedUrl);
            var responseString = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GoogleUserInfoResult>(responseString);
        }
    }
}