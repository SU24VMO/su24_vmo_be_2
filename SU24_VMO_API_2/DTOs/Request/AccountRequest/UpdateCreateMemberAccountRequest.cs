namespace SU24_VMO_API.DTOs.Request.AccountRequest
{
    public class UpdateCreateMemberAccountRequest
    {
        public Guid? CreateMemberRequestID { get; set; } = default!;
        public Guid RequestManagerId { get; set; }
        public bool? IsApproved { get; set; } = default!;
    }
}
