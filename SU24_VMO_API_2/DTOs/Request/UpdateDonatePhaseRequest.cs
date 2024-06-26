namespace SU24_VMO_API_2.DTOs.Request
{
    public class UpdateDonatePhaseRequest
    {
        public Guid DonatePhaseId { get; set; } = default!;
        public Guid AccountId { get; set; }
        public string? Name { get; set; } = default!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
