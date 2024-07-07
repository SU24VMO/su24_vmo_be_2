using Newtonsoft.Json;

namespace SU24_VMO_API_2.DTOs.Response
{
    public class ApiResponse
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("desc")]
        public string Description { get; set; }

        [JsonProperty("data")]
        public Data2 Data { get; set; }
    }

    public class Data2
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }

        [JsonProperty("expiresAt")]
        public long ExpiresAt { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
