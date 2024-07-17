namespace SU24_VMO_API_2.DTOs.Request
{
    public class UpdateStatusCreateOrganizationManagerVerifiedRequest
    {
        public Guid CreateOrganizationManagerRequestID { get; set; } = default!;
        public bool IsDisable { get; set; } = default!;
    }
}
