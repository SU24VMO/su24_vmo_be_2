namespace SU24_VMO_API_2.DTOs.Request
{
    public class UpdateStatusCreateVolunteerAccountRequest
    {
        public Guid CreateVolunteerRequestID { get; set; } = default!;
        public bool IsDisable { get; set; } = default!;
    }
}
