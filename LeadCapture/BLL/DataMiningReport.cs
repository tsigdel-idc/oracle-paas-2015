using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using IDC.Common;
using IDC.LeadCapture.DAL;

namespace IDC.LeadCapture.BLL
{
    public class DataMiningReport
    {
        CsvExport csv = null;
        ReportsRepo _db = null;
        ScoringModel sm = null;

        private string _assessmentName = null;
        private string _viewName = null;
        private string _baseUrl = null;

        private const string timeFormat = "yyyy-MM-dd HH:mm:ss";

        private string col1 = ResourceCache.Localize("csv_dm_Timestamp");
        private string col2 = ResourceCache.Localize("csv_dm_Company Name");
        private string col3 = ResourceCache.Localize("csv_dm_First Name");
        private string col4 = ResourceCache.Localize("csv_dm_Last Name");
        private string col5 = ResourceCache.Localize("csv_dm_Email");
        private string col6 = ResourceCache.Localize("csv_dm_Address");
        private string col7 = ResourceCache.Localize("csv_dm_City");
        private string col8 = ResourceCache.Localize("csv_dm_State");
        private string col9 = ResourceCache.Localize("csv_dm_Zip");
        private string col10 = ResourceCache.Localize("csv_dm_Country");
        private string col11 = ResourceCache.Localize("csv_dm_JobFunction");
        private string col12 = ResourceCache.Localize("csv_dm_Industry");
        private string col13 = ResourceCache.Localize("csv_dm_EmployeeSize");
        private string col14 = ResourceCache.Localize("csv_dm_DTM");
        private string col15 = ResourceCache.Localize("csv_dm_SourceID");
        private string col16 = ResourceCache.Localize("csv_dm_SourceDesc");
        private string col17 = ResourceCache.Localize("csv_dm_SourceLink");

        public DataMiningReport(string assessmentName, string viewName, string baseUrl)
        {
            csv = new CsvExport();
            _db = new ReportsRepo();
            sm = new ScoringModel();

            _assessmentName = assessmentName;
            _viewName = viewName;
            _baseUrl = baseUrl ?? string.Empty;

            csv.AddRow();
            csv["1"] = col1;
            csv["2"] = col2;
            csv["3"] = col3;
            csv["4"] = col4;
            csv["5"] = col5;
            csv["6"] = col6;
            csv["7"] = col7;
            csv["8"] = col8;
            csv["9"] = col9;
            csv["10"] = col10;
            csv["11"] = col11;
            csv["12"] = col12;
            csv["13"] = col13;
            csv["14"] = col14;
            csv["15"] = col15;
            csv["16"] = col16;
            csv["17"] = col17;
        }

        public string ToCSV(string culture, DateTime startDate, DateTime endDate)
        {
            var list = _db.GetReport(_assessmentName, _viewName, culture, startDate, endDate);

            foreach(var assessment in list)
            {
                csv.AddRow();
                csv["1"] = assessment.DateCreated.ToString(timeFormat);

                if (assessment.Score.HasValue)
                {
                    int score = sm.GetRoundScore((decimal)assessment.Score);
                    csv["14"] = sm.GetAltLevelName(score);
                }
                else
                {
                    csv["14"] = string.Empty;
                }

                if (assessment.CampaignId > 0)
                {
                    csv["15"] = assessment.CampaignId.ToString();
                    csv["16"] = assessment.CampaignDescription ?? string.Empty;
                    csv["17"] = assessment.CampaignGuid == Guid.Empty ? string.Empty : _baseUrl + assessment.CampaignGuid;
                }
                else
                {
                    csv["15"] = string.Empty;
                    csv["16"] = string.Empty;
                    csv["17"] = string.Empty;
                }

                foreach(var question in assessment.Questions)
                {
                    foreach(var answer in question.Answers)
                    {
                        if (question.Text == col2) csv["2"] = answer.Value;
                        else if (question.Text == col3) csv["3"] = answer.Value;
                        else if (question.Text == col4) csv["4"] = answer.Value;
                        else if (question.Text == col5) csv["5"] = answer.Value;
                        else if (question.Text == col6) csv["6"] = answer.Value;
                        else if (question.Text == col7) csv["7"] = answer.Value;
                        else if (question.Text == col8) csv["8"] = answer.Value;
                        else if (question.Text == col9) csv["9"] = answer.Value;
                        else if (question.Text == col10) csv["10"] = answer.Text;
                        else if (question.Text == col11) csv["11"] = answer.Text;
                        else if (question.Text == col12) csv["12"] = answer.Text;
                        else if (question.Text == col13) csv["13"] = answer.Text;
                    }
                }
            }

            return csv.Export();
        }
    }
}