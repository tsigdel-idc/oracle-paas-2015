using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IDC.LeadCapture.Models.Admin
{
    public enum CampaignStatus { Active, Deleted };

    public class Campaign
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public Guid Guid { get; set; }
        public string Link { get; set; }
        public bool Disabled { get; set; }
        public CampaignStatus Status { get; set; }
    }
}