namespace SU24_VMO_API_2.DTOs.Request
{
    public class UpdateCreateActivityRequestRequest
    {
        public string? Title { get; set; } = default!;
        public string? Content { get; set; } = default!;
        public List<IFormFile>? ActivityImages { get; set; } = default!;
        public List<IFormFile>? ProcessingPhaseStatementFiles { get; set; } = default!;
    }
}
