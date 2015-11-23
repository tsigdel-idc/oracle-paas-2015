using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IDC.LeadCapture.Models.Assessment
{
    public enum AnswerType
    {
        Undefined,
        Checkbox,
        Radio,
        DropDown,
        Text,
        Integer
    }

    // allows multiple style versions of the same view
    public enum ViewStyle
    {
        Style1,
        Style2,
        Style3
    }

    public class Assessment
    {
        public Assessment()
        {
            Questions = new List<Question>();
        }

        public string Name { get; set; }
        public long ResponseId { get; set; }
        public string ResponseKey { get; set; }
        public int CurrentPageNo { get; set; }
        public int TotalPages { get; set; }
        public decimal? Score { get; set; }
        public DateTime DateCreated { get; set; }

        // campaign
        public long CampaignId { get; set; }
        public Guid CampaignGuid { get; set; }
        public string CampaignName { get; set; }
        public string CampaignDescription { get; set; }

        public List<Question> Questions { get; set; }
    }
}