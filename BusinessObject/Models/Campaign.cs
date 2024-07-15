using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Campaign
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
        public bool IsDisable { get; set; } = default!;
        public bool CanBeDonated { get; set; } = default!;

        public DateTime? CheckTransparentDate { get; set; }
        public string? Note { get; set; } = default!;

        public Organization? Organization { get; set; }
        public CampaignType? CampaignType { get; set; }
        public DonatePhase? DonatePhase { get; set; }
        public ProcessingPhase? ProcessingPhase { get; set; }
        public StatementPhase? StatementPhase { get; set; }
        public CreateCampaignRequest? CreateCampaignRequest { get; set; }

        public List<Transaction>? Transactions { get; set; }

    }
}
