﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Organization
    {
        public Guid OrganizationID { get; set; } = default!;
        public Guid OrganizationManagerID { get; set;} = default!;
        public string? Name { get; set; } = default!;
        public string? Logo { get; set; } = default!;
        public string? Description { get; set; } = default!;
        public string? Website { get; set; } = default!;
        public string? Tax {  get; set; } = default!;
        public string? Location { get; set; } = default!;
        public DateTime? FoundingDate { get; set; } = default!;
        public string? OperatingLicense { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool? IsActive { get; set; } = default!;
        public bool? IsModify { get; set; } = default!;
        public bool IsDisable { get; set; } = default!;
        public string? Category { get; set; } = default!;
        public string? Note { get; set; } = default!;
        public virtual CreateOrganizationRequest? CreateOrganizationRequest { get; set; }
        public virtual OrganizationManager? OrganizationManager { get; set; }
        public virtual List<Achievement>? Achievements { get; set; }
        public virtual List<Campaign>? Campaigns { get; set; }
    }
}
