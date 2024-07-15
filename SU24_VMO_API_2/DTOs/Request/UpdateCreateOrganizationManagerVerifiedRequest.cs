namespace SU24_VMO_API_2.DTOs.Request
{
    public class UpdateCreateOrganizationManagerVerifiedRequest
    {
        public string? Name { get; set; } = default!;
        public string? PhoneNumber { get; set; } = default!;
        public string? Email { get; set; } = default!;
        public string? Address { get; set; } = default!;
        public string? CitizenIdentification { get; set; } = default!;
        public string? PersonalTaxCode { get; set; } = default!;
        public bool? IsAcceptTermOfUse { get; set; } = default!;
    }
}
