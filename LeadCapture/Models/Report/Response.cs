using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IDC.LeadCapture.Models
{
    public class Response
    {
        public long Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string Email { get; set; }
        public long CampaignId { get; set; }
        public int CompletedPageNo { get; set; }
        public bool Completed { get; set; }
        public bool ReportSent { get; set; }
    }
}