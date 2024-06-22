﻿namespace SU24_VMO_API.DTOs.Request
{
    public class AcceptOrRejectCreateOrganizationRequestRequest
    {
        public Guid? CreateOrganizationRequestID { get; set; } = default!;
        public Guid RequestManagerId { get; set; }
        public bool? IsApproved { get; set; } = default!;
    }
}
