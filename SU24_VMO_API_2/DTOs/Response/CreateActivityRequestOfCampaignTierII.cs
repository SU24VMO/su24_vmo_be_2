﻿using BusinessObject.Models;

namespace SU24_VMO_API_2.DTOs.Response
{
    public class CreateActivityRequestOfCampaignTierII
    {
        public List<CreateActivityRequest> CreateActivityRequests { get; set; }
        public int TotalItem { get; set; }
    }
}
