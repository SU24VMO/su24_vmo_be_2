using BusinessObject.Models;

namespace SU24_VMO_API_2.DTOs.Response
{
    public class CampaignResponse
    {
        public Guid CampaignID { get; set; } = default!;
        public Guid? OrganizationID { get; set; } = default!;
        public Guid CampaignTypeID { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string? Name { get; set; } = default!;
        public string? Description { get; set; } = default!;
        public string? Image { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime ExpectedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public string TargetAmount { get; set; } = default!;
        public string? ApplicationConfirmForm { get; set; } = default!;
        public bool IsTransparent { get; set; } = default!;
        public DateTime CreateAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = default!;
        public bool IsModify { get; set; } = default!;
        public bool IsComplete { get; set; } = default!;
        public bool CanBeDonated { get; set; } = default!;

        public DateTime? CheckTransparentDate { get; set; }
        public string? Note { get; set; } = default!;

        public Organization? Organization { get; set; }
        public CampaignType? CampaignType { get; set; }
        public DonatePhase? DonatePhase { get; set; }
        public ProcessingPhase? ProcessingPhase { get; set; }
        public StatementPhase? StatementPhase { get; set; }
        public User? User { get; set; } 
        public List<Transaction>? Transactions { get; set; }
    }
}
