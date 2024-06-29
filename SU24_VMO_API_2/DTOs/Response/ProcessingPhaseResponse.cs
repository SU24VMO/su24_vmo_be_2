namespace SU24_VMO_API_2.DTOs.Response
{
    public class ProcessingPhaseResponse
    {
        public Guid ProcessingPhaseId { get; set; }
        public string? CampaignName { get; set; } = default!;
    }
}
