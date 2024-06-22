namespace SU24_VMO_API.DTOs.Request
{
    public class UpdateDonatePhaseStatusRequest
    {
        public Guid DonatePhaseId { get; set; } = default!;
        public bool IsEnd { get; set; } = default!;
    }
}
