namespace SU24_VMO_API.DTOs.Request
{
    public class CreateOrganizationManagerVerifiedRequest
    {
        public Guid OrganizationManagerID { get; set; } = default!;
        public string? Name { get; set; } = default!;
        public string? PhoneNumber { get; set; } = default!;
        public string? Address { get; set; } = default!;
        public string? CitizenIdentification { get; set; } = default!;
        public string? PersonalTaxCode { get; set; } = default!;
        public bool IsAcceptTermOfUse { get; set; } = default!;

    }
}
