namespace SU24_VMO_API.DTOs.Request.AccountRequest
{
    public class UpdateActivityRequest
    {
        public Guid ActivityId { get; set; } = default!;
        public string? Title { get; set; } = default!;
        public string? Content { get; set; } = default!;
    }
}
