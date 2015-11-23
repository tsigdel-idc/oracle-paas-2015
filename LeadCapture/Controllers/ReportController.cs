using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Globalization;
using System.Text;
using System.IO;
using IDC.Common;
using IDC.LeadCapture.BLL;
using IDC.LeadCapture.DAL;

namespace IDC.LeadCapture.Controllers
{
    [Authorize(Roles = "Admin, Master")]
    public class ReportController : Controller
    {
        private string _csvReportViewName = ConfigurationManager.AppSettings["CsvReportViewName"];
        private string _dataMiningViewName = ConfigurationManager.AppSettings["DataMiningViewName"];

        private string _csvReportId = ConfigurationManager.AppSettings["CsvReportId"];
        private string _cultureName = CultureInfo.CurrentUICulture.TextInfo.CultureName;
        private CultureInfo _culture_US = CultureInfo.CreateSpecificCulture("en-US");
        private const string dateFormat = "yyyyMMdd";

        private static string _csvReportFilename = ConfigurationManager.AppSettings["CsvReportFilename"];

        [HttpGet]
        [AllowAnonymous]
        public ActionResult EmailCsv(string id, string numberofdays, string dayofweek)
        {
            if (id != _csvReportId) return null;

            string msg = null;
            bool success = false;
            var today = DateTime.Today;

            try
            {
                int dof = 0;
                if (!string.IsNullOrEmpty(dayofweek) && !int.TryParse(dayofweek, out dof))
                {
                    msg = "Invalid dayofweek parameter: " + dayofweek;
                    Logger.Log(LogLevel.Error, msg);
                    Response.Write(msg);
                    return null;
                }

                // date to execute: 0 today, 1 Sunday, 2 Monday, 3 Tuesday, 4 Wednesday, 5 Thursday, 6 Friday, 7 Saturday
                if (dof != 0 && dof != (int)today.DayOfWeek + 1) return null;

                // number of days csv report will include in the data, default to 7 days
                int days = 7;
                if (!string.IsNullOrEmpty(numberofdays) && !int.TryParse(numberofdays, out days))
                {
                    msg = "Invalid numberofdays parameter: " + numberofdays;
                    Logger.Log(LogLevel.Error, msg);
                    Response.Write(msg);
                    return null;
                }

                var startDate = today.AddDays(-days);

                var report = new CsvReport(QuestionCache.AssessmentName, _csvReportViewName);
                string csv = report.ToCSV(_cultureName, startDate, today);
                string fileName = FormatCsvReportFileName(startDate, today.AddDays(-1));

                var smtpMail = new SmtpMail();
                success = smtpMail.SendCsvReport(csv, fileName);
            }
            catch (Exception e)
            {
                msg = e.Message;
                Logger.Log(LogLevel.Error, msg);
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);                
            }

            if (success)
            {
                Response.Write("Success");
            }
            else
            {
                if (msg == null) msg = "Failed to send email";
                Response.Write(msg);
            }

            return null;
        }

        [HttpGet]
        public void Pdf(string id)
        {
            // Validate arguments
            if (string.IsNullOrEmpty(id))
            {
                Response.StatusCode = 403;  // Forbidden
                Response.End();
                return;
            }

            // Parse response ID from URL
            int responseId = 0;
            if (!int.TryParse(id, out responseId))
            {
                Response.StatusCode = 400;  // Bad request
                Response.End();
                return;
            }

            if (responseId <= 0)
            {
                Response.StatusCode = 403;  // Forbidden
                Response.End();
                return;
            }

            DownloadPdfReport(responseId);
        }

        #region dashboard reports

        [HttpGet]
        public ActionResult DataMining()
        {
            return View();
        }

        [HttpGet]
        public void DataMiningReport(string daterange)
        {
            var startDate = DateTime.MinValue;
            var endDate = DateTime.Today;

            if (!string.IsNullOrEmpty(daterange))
            {
                DateTime date;
                var parts = daterange.Split('-');
                if (parts.Length > 0 && DateTime.TryParse(parts[0], _culture_US, DateTimeStyles.None, out date)) startDate = date;
                if (parts.Length > 1 && DateTime.TryParse(parts[1], _culture_US, DateTimeStyles.None, out date)) endDate = date;
            }

            DownloadDataMiningReport(startDate, endDate);
        }

        [HttpGet]
        public ActionResult UsageReport()
        {
            var report = new UsageReport();
            var model = report.GetReport();
            return View(model);
        }

        #endregion

        #region heplers

        private string FormatCsvReportFileName(DateTime startDate, DateTime endDate)
        {
            return string.Format(_csvReportFilename, startDate.ToString(dateFormat), endDate.ToString(dateFormat));
        }

        private void DownloadPdfReport(int responseId)
        {
            try
            {
                var db = new AssessmentRepo();
                var model = db.GetAnswers(QuestionCache.AssessmentName, _cultureName, responseId);
                var scoringModel = new ScoringModel();
                var report = scoringModel.GetReport(model);

                using (var ms = new System.IO.MemoryStream())
                {
                    // create report
                    var pdf = new PdfReport();
                    pdf.GenerateReport(report, ms, null);

                    // Send response to browser
                    Response.Clear();
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    //HttpContext.Current.Response.ContentType = "pdf/application";                                       // Causes the pdf file to download rather than display in browser
                    Response.ContentType = "application/pdf";                                                             // Causes the pdf file to display directly in browser   
                    Response.AddHeader("content-disposition", "inline;filename=\"" + SmtpMail.ReportFilename + "\"");   // Filename is required if downloading rather than displaying pdf     
                    Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                    Response.Flush();
                    Response.End();
                    Response.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, e.Message);
            }
        }

        private void DownloadDataMiningReport(DateTime startDate, DateTime endDate)
        {
            string fileName = FormatCsvReportFileName(startDate, endDate);
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";

            try
            {
                var report = new DataMiningReport(QuestionCache.AssessmentName, _dataMiningViewName, baseUrl);
                string csv = report.ToCSV(_culture_US.TextInfo.CultureName, startDate, endDate.AddDays(1));

                StringWriter writer = new StringWriter();
                writer.WriteLine(csv);
                Response.ContentType = "text/csv; charset=UTF-8";

                Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                Response.Clear();

                using (StreamWriter stream = new StreamWriter(Response.OutputStream, Encoding.UTF8))
                {
                    stream.Write(writer.ToString());
                }

                Response.End();
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, e.Message);
            }
        }

        #endregion
    }
}
