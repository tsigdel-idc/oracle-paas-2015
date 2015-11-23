using System;
using System.Web.Security;
using IDC.LeadCapture;
using IDC.LeadCapture.DAL;

namespace IDC.Extensions
{
    public class FormsAuthenticationHelper
    {
        AccountRepo _db = new AccountRepo();

        public LeadCapture.Models.Account.UserStatus Authenticate(string username, string password, bool rememberMe)
        {
            var status = LeadCapture.Models.Account.UserStatus.Undefined;
            bool result = _db.Authenticate(username, password, out status);
            if (result) FormsAuthentication.SetAuthCookie(username, rememberMe);
            else status = LeadCapture.Models.Account.UserStatus.Undefined;

            return status;
        }

        public void SignOut()
        {
            // Clear UserName from cookie and sign out
            FormsAuthentication.SignOut();
        }
    }
}