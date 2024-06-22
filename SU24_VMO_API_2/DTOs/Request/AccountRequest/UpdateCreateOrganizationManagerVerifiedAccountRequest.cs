namespace SU24_VMO_API.DTOs.Request.AccountRequest
{
    public class UpdateCreateOrganizationManagerVerifiedAccountRequest
    {
        public Guid? CreateOrganizationManagerRequestID { get; set; } = default!;
        public bool? IsApproved { get; set; } = default!;
    }
}
