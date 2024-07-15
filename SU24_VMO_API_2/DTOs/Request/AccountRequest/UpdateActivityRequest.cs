using BusinessObject.Models;

namespace SU24_VMO_API.DTOs.Request.AccountRequest
{
    public class UpdateActivityRequest
    {
        public string? Title { get; set; } = default!;
        public string? Content { get; set; } = default!;
        public List<IFormFile>? ActivityImages { get; set; }
    }
}
