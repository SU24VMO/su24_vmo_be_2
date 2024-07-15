namespace SU24_VMO_API.DTOs.Request
{
    public class UpdateOrganizationRequest
    {
        public string? Name { get; set; } = default!;
        public IFormFile? Logo { get; set; } = default!;
        public string? Description { get; set; } = default!;
        public string? Website { get; set; } = default!;
        public string? Tax { get; set; } = default!;
        public string? Location { get; set; } = default!;
        public DateTime? FoundingDate { get; set; } = default!;
        public string? OperatingLicense { get; set; } = default!;
        public string? Category { get; set; } = default!;
        public string? Note { get; set; } = default!;
    }
}
