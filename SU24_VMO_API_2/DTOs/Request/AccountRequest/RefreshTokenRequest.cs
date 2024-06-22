using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API.DTOs.Request.AccountRequest
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = default!;
    }
}
