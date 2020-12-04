using System;
using Newtonsoft.Json;

namespace WaterMelon_API.Models
{
    public partial class GoogleUserInfoResult
    {
        [JsonProperty("iss")]
        public string Iss { get; set; }

        [JsonProperty("azp")]
        public string Azp { get; set; }

        [JsonProperty("aud")]
        public string Aud { get; set; }

        [JsonProperty("sub")]
        public string Sub { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("email_verified")]
        public bool EmailVerified { get; set; }

        [JsonProperty("at_hash")]
        public string AtHash { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("picture")]
        public Uri Picture { get; set; }

        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("iat")]
        public long Iat { get; set; }

        [JsonProperty("exp")]
        public long Exp { get; set; }

        [JsonProperty("jti")]
        public string Jti { get; set; }

        [JsonProperty("alg")]
        public string Alg { get; set; }

        [JsonProperty("kid")]
        public string Kid { get; set; }

        [JsonProperty("typ")]
        public string Typ { get; set; }
    }
}