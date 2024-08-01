using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API.DTOs.Request.AccountRequest
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = default!;
        public string? Longitude { get; set; }
        public string? Latitude { get; set; }
        public string? Road { get; set; }
        public string? Suburb { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Postcode { get; set; }
        public string? CountryCode { get; set; }
    }
}
