using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IDC.LeadCapture.Models
{
    public class Usage
    {
        private int[] _data = new int[12];
        private string[] _labels = new string[12];

        // Chart data: 165, 159, 280, 181, 156, 155, 140, 156, 155, 140, 90, 40
        public int[] Data { get { return _data; } }


        // Chart labels: "2015/Jan", "2015/Feb", "2015/Mar", "2015/Apr", "2015/May", "2015/Jun", "2015/Jul", "2015/Aug", "2015/Sep", "2015/Oct", "2015/Nov", "2015/Dec"
        public string[] Labels { get { return _labels; } }

        // Assessments
        public int TotalAssessments { get; set; }
        public int CompletedAssessments { get; set; }
        public int IncompleteAssessments { get; set; }
        public int NotStartedAssessments { get; set; }

        // Users
        public int TotalUsers { get; set; }
        public int ReportedAssessments { get; set; }

        // Lead Sources
        public int TotalLeadSources { get; set; }
        public int ActiveLeadSources { get; set; }
        public int DisabledLeadSources { get; set; }
        public int UsedLeadSources { get; set; }
    }
}