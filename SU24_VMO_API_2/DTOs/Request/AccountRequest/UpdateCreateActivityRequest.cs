﻿namespace SU24_VMO_API.DTOs.Request.AccountRequest
{
    public class UpdateCreateActivityRequest
    {
        public Guid CreateActivityRequestId {  get; set; }
        public Guid RequestManagerId { get; set; }
        public bool IsAccept {  get; set; }
    }
}