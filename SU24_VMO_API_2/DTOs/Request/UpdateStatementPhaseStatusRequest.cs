namespace SU24_VMO_API_2.DTOs.Request
{
    public class UpdateStatementPhaseStatusRequest
    {
        public Guid StatementPhaseId { get; set; } = default!;
        public bool IsEnd { get; set; } = default!;
        public Guid AccountId { get; set; } = default!;

    }
}
