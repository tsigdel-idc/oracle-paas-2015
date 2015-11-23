using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net.Mime;
using System.Configuration;
using System.IO;
using System.Text;
using IDC.Common;

namespace IDC.LeadCapture.BLL
{
    public class SmtpMail
    {
        private static string _supportEmailAddress = ConfigurationManager.AppSettings["SupportEmailAddress"];
        private static string _fromAddress = ConfigurationManager.AppSettings["ReportEmailFromAddress"];
        
        // PDF Report
        private static string _reportEmail_Subject = ConfigurationManager.AppSettings["ReportEmailSubject"];
        private static string _reportFilename = ConfigurationManager.AppSettings["ReportFilename"];

        #region report email template

        private static string _reportEmail_Body = ResourceCache.Localize("report_email_txt");
        private string _attachment = MapLocalizedPath("~/Content/pdf/", "pdf_attachment_name");

        #endregion

        // reset password
        // IDC Digital Transformation MaturityScape - Your Password Has Been Reset
        private static string _emailSubject_ResetPassword = ResourceCache.Localize("emailSubject_ResetPassword");

        private static string _emailBody_ResetPassword = ResourceCache.Localize("emailBody_ResetPassword");
        // A temporary password has been created for your account: {0} (case sensitive)<br/><br/>
        // Please go to {1} to reset your password.<br/><br/><br/>
        // P.S. Please do not respond to this automated message. This account is not monitored.

        private const string _url_resetPassword = @"{0}/account/changepassword/{1}/{2}";

        // CSV Report
        private static string _csvReportEmail_Subject = ConfigurationManager.AppSettings["CsvReportEmailSubject"];
        private static string _csvReportEmail = ConfigurationManager.AppSettings["CsvReportEmail"];

        // send synchronous messages
        private void SendMail(MailMessage msg)
        {
#if DEBUG
            var file = new System.IO.StreamWriter("c:\\EmailMsg.txt", true);
            file.WriteLine(DateTime.Now);
            file.WriteLine(msg.Subject);
            file.WriteLine(msg.Body);
            file.WriteLine();
            file.Close();
            return;
#endif
            var smtpClient = new SmtpClient();

            try
            {
                smtpClient.Send(msg);
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, e.Message);
                throw new Exception(e.Message);
            }
        }

        public bool SendReport(Models.Scoring report)
        {
            bool success = false;
            string email = report.Email;

            var msg = new MailMessage();
            msg.IsBodyHtml = true;
            msg.Subject = _reportEmail_Subject;
            msg.From = new MailAddress(_fromAddress);
            msg.To.Add(email);
            
            string htmlBody = string.Format(_reportEmail_Body, report.CompanyName);

            msg.Body = htmlBody;

            using (var ms = new System.IO.MemoryStream())
            {
                try
                {
                    //var pdf = new PdfReport();
                    //pdf.GenerateReport(report, ms, null);

                    // attach dynamic report to email
                    //var attachment = new Attachment(ms, _reportFilename, MediaTypeNames.Application.Pdf);
                    //msg.Attachments.Add(attachment);

                    // attach static pdf file
                    if (_attachment != null) msg.Attachments.Add(new Attachment(_attachment, MediaTypeNames.Application.Pdf));

                    // send email
                    SendMail(msg);
                    success = true;
                    Logger.Log(LogLevel.Trace, "Email sent to: " + email);
                }
                catch (Exception)
                {
                    Logger.Log(LogLevel.Error, "Failed to send email to: " + email);
                }

                return success;
            }
        }

        public bool SendCsvReport(string contents, string filename)
        {
            bool success = false;

            var msg = new MailMessage();
            msg.IsBodyHtml = true;
            msg.Subject = _csvReportEmail_Subject;
            msg.From = new MailAddress(_fromAddress);
            msg.Body = _csvReportEmail_Subject;

            // add recipients
            string[] items = _csvReportEmail.Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach(var item in items)
            {
                string[] parts = item.Split(new Char[] { '(' }, StringSplitOptions.RemoveEmptyEntries);
                string email = null;
                string displayName = null;
                if (parts.Length > 0)
                {
                    email = parts[0];
                    if (parts.Length > 1) displayName = parts[1];
                }

                if (!string.IsNullOrEmpty(email))
                {
                    if (!string.IsNullOrEmpty(displayName))
                    {
                        displayName = displayName.Replace("(", string.Empty).Replace(")", string.Empty);
                        msg.To.Add(new System.Net.Mail.MailAddress(email.Trim(), displayName.Trim()));
                    }
                    else
                    {
                        msg.To.Add(new System.Net.Mail.MailAddress(email.Trim()));
                    }
                }
            }

            // add csv file attachment
            byte[] byteArray = Encoding.UTF8.GetBytes(contents);

            using (var ms = new System.IO.MemoryStream(byteArray))
            {
                try
                {
                    var attachment = new Attachment(ms, filename, MediaTypeNames.Text.Plain);
                    msg.Attachments.Add(attachment);

                    // send email
                    SendMail(msg);
                    success = true;
                    Logger.Log(LogLevel.Trace, "CsvReport email sent to: " + _csvReportEmail);
                }
                catch (Exception)
                {
                    Logger.Log(LogLevel.Error, "Failed to send CsvReport email to: " + _csvReportEmail);
                }
            }

            return success;
        }

        public void SendPasswordResetEmail(Models.Account.User model, string urlAuthority)
        {
            string email = model.Email;
            string tempPassword = model.NewPassword;

            string displayName;
            string firstName = model.FirstName;
            string lastName = model.LastName;

            string url = string.Format(_url_resetPassword, urlAuthority, model.Guid, model.UserName);

            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
            {
                displayName = string.Empty;
            }
            else
            {
                displayName = (Helper.ToTitleCase(firstName) + " " + Helper.ToTitleCase(lastName)).Trim();
            }

            // Including the display name as well as the e-mail address makes the message more likely to be delivered
            var to = new System.Net.Mail.MailAddress(email, displayName);
            var from = new System.Net.Mail.MailAddress(_fromAddress);

            var mail = new System.Net.Mail.MailMessage();
            mail.IsBodyHtml = true;
            mail.To.Add(to);
            mail.From = from;
            mail.Subject = _emailSubject_ResetPassword;
            mail.Body = string.Format(_emailBody_ResetPassword, tempPassword, url);

            BLL.SmtpMail smtp = new BLL.SmtpMail();
            smtp.SendMail(mail);
            Logger.Log(LogLevel.Trace, "PasswordReset email sent to: " + to);
        }

        private static string MapLocalizedPath(string relativePath, string key)
        {
            string localizedPath = ResourceCache.Localize(key);
            if (string.IsNullOrEmpty(localizedPath)) return null;
            return HttpContext.Current.Server.MapPath(Path.Combine(relativePath, localizedPath));
        }

        #region static properties

        public static string ReportFilename { get { return _reportFilename; } }
        public static string SupportEmailAddress { get { return _supportEmailAddress; } }

        #endregion
    }
}