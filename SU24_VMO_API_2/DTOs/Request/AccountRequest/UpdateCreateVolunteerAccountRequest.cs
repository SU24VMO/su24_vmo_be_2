namespace SU24_VMO_API.DTOs.Request.AccountRequest
{
    public class UpdateCreateVolunteerAccountRequest
    {
        public Guid? CreateVolunteerRequestID { get; set; } = default!;
        public Guid ModeratorId { get; set; }
        public bool? IsApproved { get; set; } = default!;
    }
}
