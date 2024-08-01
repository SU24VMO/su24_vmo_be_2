using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API.DTOs.Request.AccountRequest
{
    public class LoginRequest
    {
        public string? Account { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;
        public string? Longitude { get; set; } = default!;
        public string? Latitude { get; set; } = default!;
        public string? Road { get; set; } = default!;
        public string? Suburb { get; set; } = default!;
        public string? City { get; set; } = default!;
        public string? Country { get; set; } = default!;
        public string? Postcode { get; set; } = default!;
        public string? CountryCode { get; set; } = default!;
    }
}
