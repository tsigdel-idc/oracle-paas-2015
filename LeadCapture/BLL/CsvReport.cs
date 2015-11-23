using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using IDC.Common;
using IDC.LeadCapture.DAL;

namespace IDC.LeadCapture.BLL
{
    public class CsvReport
    {
        CsvExport csv = null;
        ReportsRepo _db = null;
        ScoringModel sm = null;

        private string _assessmentName = null;
        private string _viewName = null;
        private const string timeFormat = "yyyy-MM-dd HH:mm:ss";

        private string col1 = ResourceCache.Localize("csv_Timestamp");
        private string col2 = ResourceCache.Localize("csv_Company Name");
        private string col3 = ResourceCache.Localize("csv_First Name");
        private string col4 = ResourceCache.Localize("csv_Last Name");
        private string col5 = ResourceCache.Localize("csv_Email");
        private string col6 = ResourceCache.Localize("csv_Address");
        private string col7 = ResourceCache.Localize("csv_City");
        private string col8 = ResourceCache.Localize("csv_State");
        private string col9 = ResourceCache.Localize("csv_Zip");
        private string col10 = ResourceCache.Localize("csv_Country");
        private string col11 = ResourceCache.Localize("csv_DTM");


        public CsvReport(string assessmentName, string viewName)
        {
            csv = new CsvExport();
            _db = new ReportsRepo();
            sm = new ScoringModel();

            _assessmentName = assessmentName;
            _viewName = viewName;

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
        }

        public string ToCSV(string culture, DateTime startDate, DateTime endDate)
        {
            var list = _db.GetReport(_assessmentName, _viewName, culture, startDate, endDate);

            foreach(var assessment in list)
            {
                csv.AddRow();
                csv["1"] = assessment.DateCreated.ToString(timeFormat);

                int score = sm.GetRoundScore(assessment.Score.HasValue ? (decimal)assessment.Score : 0);
                csv["11"] = sm.GetAltLevelName(score);

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
                    }
                }
            }

            return csv.Export();
        }

    }
}
