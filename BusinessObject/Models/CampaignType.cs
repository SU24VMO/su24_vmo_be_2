using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class CampaignType
    {
        public Guid CampaignTypeID { get; set; } = default!;
        public string? Name { get; set; } = default!;
        public DateTime CreateAt { get; set; }
        public bool IsValid { get; set; } = default!;

        public List<Campaign>? Campaigns { get; set; }
    }
}
