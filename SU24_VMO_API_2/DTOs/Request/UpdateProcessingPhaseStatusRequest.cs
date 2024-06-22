namespace SU24_VMO_API.DTOs.Request
{
    public class UpdateProcessingPhaseStatusRequest
    {
        public Guid ProcessingPhaseId { get; set; } = default!;
        public bool IsEnd { get; set; } = default!;
    }
}
