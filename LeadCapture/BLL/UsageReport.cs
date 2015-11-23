using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using IDC.LeadCapture.Models;
using IDC.LeadCapture.DAL;

namespace IDC.LeadCapture.BLL
{
    public class UsageReport
    {
        ReportsRepo _db = new ReportsRepo();
        private const string dateFormat = "yyyy/MMM";

        public Models.Usage GetReport()
        {
            var usage = new Models.Usage();

            int length = usage.Labels.Length;   // 12
            var usageData = new Dictionary<string, int>();
            var emails = new List<string>();
            var campaignIdList = new List<long>();

            var today = DateTime.Today;
            var endDate = today.AddDays(1 - today.Day).AddMonths(1);
            var startDate = endDate.AddMonths(-length);

            for (int i = 0; i < usage.Labels.Length; i++)
            {
                string month = startDate.AddMonths(i).ToString(dateFormat, CultureInfo.CurrentUICulture);
                usage.Labels[i] = month;
                if (!usageData.ContainsKey(month)) usageData.Add(month, 0);
            }

            var responses = _db.GetResponses(startDate, endDate);

            foreach (var item in responses)
            {
                string month = item.DateCreated.ToString(dateFormat, CultureInfo.CurrentUICulture);

                if (usageData.ContainsKey(month)) 
                {
                    int val = usageData[month];
                    usageData[month] = ++val;
                }

                usage.TotalAssessments++;

                if (item.Completed)
                {
                    usage.CompletedAssessments++;
                    if (item.ReportSent) usage.ReportedAssessments++;
                }
                else
                {
                    usage.IncompleteAssessments++;
                    if (item.CompletedPageNo < 1) usage.NotStartedAssessments++;
                }

                if (!emails.Contains(item.Email)) emails.Add(item.Email);
                if (item.CampaignId > 0 && !campaignIdList.Contains(item.CampaignId)) usage.UsedLeadSources++;     
            }

            for (int i = 0; i < usage.Labels.Length; i++)
            {
                string month = usage.Labels[i];
                int val = usageData[month];
                usage.Data[i] = val;
            }

            usage.TotalUsers = emails.Count();

            usage.ActiveLeadSources = _db.GetActiveCampaignsCount();
            usage.DisabledLeadSources = _db.GetDisabledCampaignsCount();
            usage.TotalLeadSources = usage.ActiveLeadSources + usage.DisabledLeadSources;

            return usage;
        }
    }
}