using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using IDC.Extensions;
using IDC.Common;
using IDC.LeadCapture.Models;
using IDC.LeadCapture.DAL;
using IDC.LeadCapture.BLL;

namespace IDC.LeadCapture.Controllers
{
    [Authorize(Roles = "Guest, Admin, Master")]
    public class AccountController : Controller
    {
        FormsAuthenticationHelper _authenticationHelper = new FormsAuthenticationHelper();
        private AccountRepo _db = new AccountRepo();

        #region Authentication

        //
        // GET: /Account/Login

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (User.Identity.IsAuthenticated)
            {
                if (string.IsNullOrEmpty(returnUrl)) returnUrl = GetReturnUrl();
                if (!string.IsNullOrEmpty(returnUrl)) return Redirect(returnUrl);
                return RedirectToAction("Index", "Admin");
            }

            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Models.Account.Login model, string returnUrl)
        {
            var status = _authenticationHelper.Authenticate(model.UserName, model.Password, true);

            if (status != Models.Account.UserStatus.Undefined)
            {
                Logger.Log(LogLevel.Info, "User logged in [user name = " + model.UserName + "]");

                if (!string.IsNullOrEmpty(returnUrl)) return RedirectToLocal(returnUrl);

                if (status == Models.Account.UserStatus.Unconfirmed)
                {
                    Logger.Log(LogLevel.Info, "User redirected to ChangeTempPassword [user name = " + model.UserName + "]");
                    return Redirect(Url.Action("ChangeTempPassword", "Account"));
                }
                                
                return Redirect(Url.Action("Index", "Admin"));
            }

            // if something failed, re-display form            
            ModelState.AddModelError("Error", ResourceCache.Localize("auth-error"));
            Logger.Log(LogLevel.Error, "Authentication failed [user name = " + model.UserName + "]");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Logger.Log(LogLevel.Info, "User logged off [user name = " + User.Identity.Name + "]");
            _authenticationHelper.SignOut();
            return null;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult LogoutConfirmation()
        {
            return View();
        }

        #endregion

        #region Reset Password

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(Models.Account.User model)
        {
            model.NewPassword = CreateTempPassword(8);

            try
            {
                if (_db.ResetPassword(model))
                {
                    Logger.Log(LogLevel.Info, "Password has been reset [user name = " + model.UserName + "]");
                    string urlAuthority = Request.Url.GetLeftPart(UriPartial.Authority);
                    var smtp = new SmtpMail();
                    smtp.SendPasswordResetEmail(model, urlAuthority);
                    TempData["EmailSent"] = bool.TrueString;
                }
                else
                {
                    TempData["EmailSent"] = bool.FalseString;
                    Logger.Log(LogLevel.Error, "Password was not reset [user name = " + model.UserName + "]");
                }

                return RedirectToAction("ResetPasswordConfirmation");
            }
            catch (Exception e)
            {                
                ModelState.AddModelError("Error", ResourceCache.Localize("ui_request_not_processed"));
                Logger.Log(LogLevel.Error, "Error in ResetPassword [user name = " + model.UserName + "]: " + e.Message);
            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            if (string.IsNullOrEmpty(TempData["EmailSent"].ToString())) RedirectToAction("NotFound", "Error");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Master")]
        [ValidateAntiForgeryToken]
        public ActionResult _requestResetPassword(string userId)
        {
            string outcome = "success";
            string message = string.Empty;

            long id = 0;
            long.TryParse(userId, out id);
            var model = _db.GetUser(id);

            model.NewPassword = CreateTempPassword(8);

            try
            {
                if (_db.ResetPassword(model))
                {
                    string urlAuthority = Request.Url.GetLeftPart(UriPartial.Authority);
                    var smtp = new SmtpMail();
                    smtp.SendPasswordResetEmail(model, urlAuthority);
                    TempData["Email"] = model.Email;
                    Logger.Log(LogLevel.Info, "Password has been reset [user name = " + model.UserName + "], requested by " + User.Identity.Name);
                }
                else
                {
                    outcome = "error";
                    Logger.Log(LogLevel.Error, "Reset Password failed [user name = " + model.UserName + "], requested by " + User.Identity.Name);
                }                
            }
            catch (Exception e)
            {
                outcome = "error";
                Logger.Log(LogLevel.Error, "Error in _requestResetPassword [user name = " + model.UserName + "], requested by " +  User.Identity.Name + ": " + e.Message);
            }

            return Json(new { result = outcome, message = message }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult RequestResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ChangePasswordConfirmation()
        {
            return View();
        }

        #endregion

        #region Change Password

        [HttpGet]
        public ActionResult ChangePassword(string key, string username)
        {
            var model = _db.GetUser(key, username);
            return View(model);
        }

        [HttpGet]
        public ActionResult ChangeTempPassword()
        {
            var model = _db.GetUser(User.Identity.Name);
            return View("ChangePassword", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(Models.Account.User user)
        {
            user.UserName = User.Identity.Name;
            var status = Models.Account.UserStatus.Undefined;

            try
            {
                if (_db.Authenticate(user.UserName, user.Password, out status))
                {
                    if (_db.ChangePassword(user))
                    {
                        Logger.Log(LogLevel.Info, "Password was changed [user name = " + user.UserName + "]");
                        return RedirectToAction("ChangePasswordConfirmation");
                    }
                    else
                    {                        
                        ModelState.AddModelError("Error", ResourceCache.Localize("ui_changes_not_saved"));
                        Logger.Log(LogLevel.Info, "Error in ChangePassword: Password was not saved [user name = " + user.UserName + "]");
                    }
                }
                else
                {
                    ModelState.AddModelError("Error", ResourceCache.Localize("password-incorrect"));
                    Logger.Log(LogLevel.Error, "Error in ChangePassword: Authentication failed [user name = " + user.UserName + "]");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error", ResourceCache.Localize("error_msg"));
                Logger.Log(LogLevel.Error, "Error in ChangePassword: " + e.Message);
            }
            
            return View(user);
        }

        #endregion

        #region My Profile

        [HttpGet]
        public ActionResult MyProfile()
        {
            string userName = User.Identity.Name;
            var model = _db.GetUser(userName);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _updateUserProfile(Models.Account.User user)
        {
            string outcome = "success";
            string message = string.Empty;

            try
            {
                if (_db.UpdateUser(user))
                {
                    message = ResourceCache.Localize("ui_changes_saved");
                    Logger.Log(LogLevel.Info, "User has been updated [user name = " + user.UserName + "]");
                }
                else
                {
                    message = ResourceCache.Localize("ui_changes_not_saved");
                    Logger.Log(LogLevel.Info, "Error in _updateUserProfile: changes not saved [user name = " + user.UserName + "]");
                }
            }
            catch (Exception e)
            {
                outcome = "error";
                Logger.Log(LogLevel.Error, "Error in _updateUserProfile: " + e.Message);
            }

            return Json(new { result = outcome, message = message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _changePassword(Models.Account.User user)
        {
            string outcome = "success";
            string message = string.Empty;
            user.UserName = User.Identity.Name;
            var status = Models.Account.UserStatus.Undefined;

            try
            {
                if (_db.Authenticate(user.UserName, user.Password, out status))
                {
                    if (_db.ChangePassword(user))
                    {
                        message = ResourceCache.Localize("ui_changes_saved");
                        Logger.Log(LogLevel.Info, "Password was changed [user name = " + user.UserName + "]");
                    }
                    else
                    {
                        message = ResourceCache.Localize("ui_changes_not_saved");
                        Logger.Log(LogLevel.Info, "Error in _changePassword: Password was not saved [user name = " + user.UserName + "]");
                    }
                }
                else
                {
                    outcome = "error";
                    message = ResourceCache.Localize("password-incorrect");
                    Logger.Log(LogLevel.Error, "Error in _changePassword: Authentication failed [user name = " + user.UserName + "]");
                }
            }
            catch (Exception e)
            {
                // default error will be displayed
                outcome = "error";
                Logger.Log(LogLevel.Error, "Error in _changePassword: " + e.Message);

            }

            return Json(new { result = outcome, message = message }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region User profile

        [HttpGet]
        public ActionResult AddUser()
        {
            var model = new Models.Account.User();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(Models.Account.User user)
        {
            bool isEmailError = false;

            try
            {
                user.Password = CreateTempPassword(8);
                user.NewPassword = user.Password;
                user.Guid = Guid.NewGuid();

                // all users are Admins
                user.IsAdmin = true;

                if (_db.AddUser(user))
                {
                    isEmailError = true;
                    Logger.Log(LogLevel.Info, "User has been created [email = " + user.Email + "], requested by " + User.Identity.Name);

                    string urlAuthority = Request.Url.GetLeftPart(UriPartial.Authority);
                    var smtp = new SmtpMail();
                    smtp.SendPasswordResetEmail(user, urlAuthority);
                    isEmailError = false;

                    return RedirectToAction("ManageUsers", "Admin");
                }
                else
                {
                    ModelState.AddModelError("Error", ResourceCache.Localize("duplicate_user_msg"));
                    Logger.Log(LogLevel.Error, "Error in AddUser: duplicate user [email = " + user.Email + "], requested by " + User.Identity.Name);
                }
            }
            catch (Exception e)
            {
                if (isEmailError)
                {
                    ModelState.AddModelError("Error", ResourceCache.Localize("email_error_msg"));                    
                }
                else
                {
                    ModelState.AddModelError("Error", ResourceCache.Localize("error_msg"));
                }

                Logger.Log(LogLevel.Error, "Error in AddUser [email = " + user.Email + "], requested by " + User.Identity.Name + ": " + e.Message);
            }

            return View(user);
        }

        [HttpGet]
        public ActionResult EditUser(string id)
        {
            long userId = 0;
            long.TryParse(id, out userId);
            var model = _db.GetUser(userId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(Models.Account.User user)
        {
            try
            {
                if (_db.UpdateUser(user))
                {
                    Logger.Log(LogLevel.Info, "User has been updated [id = " + user.Id + "], requested by " + User.Identity.Name);
                    return RedirectToAction("ManageUsers", "Admin");
                }
                else
                {
                    ModelState.AddModelError("Error", ResourceCache.Localize("ui_changes_not_saved"));
                    Logger.Log(LogLevel.Error, "Error in EditUser: changes not saved [id = " + user.Id + "], requested by " + User.Identity.Name);
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error", ResourceCache.Localize("error_msg"));
                Logger.Log(LogLevel.Error, "Error in EditUser [id = " + user.Id + "], requested by " + User.Identity.Name + ": " + e.Message);
            }

            return View(user);
        }

        [HttpGet]
        public ActionResult DeleteUser(string id)
        {
            long userId = 0;
            long.TryParse(id, out userId);
            var user = _db.GetUser(userId);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUser(Models.Account.User user)
        {
            try
            {
                if (_db.DeleteUser(user))
                {
                    Logger.Log(LogLevel.Info, "User has been deleted [id = " + user.Id + "], requested by " + User.Identity.Name);
                    return RedirectToAction("ManageUsers", "Admin");
                }
                else
                {
                    ModelState.AddModelError("Error", ResourceCache.Localize("ui_changes_not_saved"));
                    Logger.Log(LogLevel.Error, "Error in DeleteUser: changes not saved [id = " + user.Id + "], requested by " + User.Identity.Name);
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error", ResourceCache.Localize("error_msg"));
                Logger.Log(LogLevel.Error, "Error in DeleteUser [id = " + user.Id + "], requested by " + User.Identity.Name + ": " + e.Message);
            }

            return View(user);
        }

        [HttpGet]
        public ActionResult RestoreUser(string id)
        {
            long userId = 0;
            long.TryParse(id, out userId);
            var user = _db.GetUser(userId);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RestoreUser(Models.Account.User user)
        {
            try
            {
                if (_db.DeleteUser(user, true))
                {
                    Logger.Log(LogLevel.Info, "User has been restored [id = " + user.Id + "], requested by " + User.Identity.Name);
                    return RedirectToAction("ManageUsers", "Admin");
                }
                else
                {
                    ModelState.AddModelError("Error", ResourceCache.Localize("ui_changes_not_saved"));
                    Logger.Log(LogLevel.Error, "Error in RestoreUser: changes not saved [id = " + user.Id + "], requested by " + User.Identity.Name);
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error", ResourceCache.Localize("error_msg"));
                Logger.Log(LogLevel.Error, "Error in RestoreUser [id = " + user.Id + "], requested by " + User.Identity.Name + ": " + e.Message);
            }

            return View(user);
        }

        [HttpGet]
        public ActionResult SuspendUser(string id)
        {
            long userId = 0;
            long.TryParse(id, out userId);
            var user = _db.GetUser(userId);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuspendUser(Models.Account.User user)
        {
            try
            {
                if (_db.SuspendUser(user))
                {
                    Logger.Log(LogLevel.Info, "User has been suspended [id = " + user.Id + "], requested by " + User.Identity.Name);
                    return RedirectToAction("ManageUsers", "Admin");
                }
                else
                {
                    ModelState.AddModelError("Error", ResourceCache.Localize("ui_changes_not_saved"));
                    Logger.Log(LogLevel.Error, "Error in SuspendUser: changes not saved [id = " + user.Id + "], requested by " + User.Identity.Name);
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error", ResourceCache.Localize("error_msg"));
                Logger.Log(LogLevel.Error, "Error in SuspendUser [id = " + user.Id + "], requested by " + User.Identity.Name + ": " + e.Message);
            }

            return View(user);
        }

        [HttpGet]
        public ActionResult UnsuspendUser(string id)
        {
            long userId = 0;
            long.TryParse(id, out userId);
            var user = _db.GetUser(userId);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnsuspendUser(Models.Account.User user)
        {
            try
            {
                if (_db.SuspendUser(user, true))
                {
                    Logger.Log(LogLevel.Info, "User has been unsuspended [id = " + user.Id + "], requested by " + User.Identity.Name);
                    return RedirectToAction("ManageUsers", "Admin");
                }
                else
                {
                    ModelState.AddModelError("Error", ResourceCache.Localize("ui_changes_not_saved"));
                    Logger.Log(LogLevel.Error, "Error in UnuspendUser: changes not saved [id = " + user.Id + "], requested by " + User.Identity.Name);
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error", ResourceCache.Localize("error_msg"));
                Logger.Log(LogLevel.Error, "Error in UnuspendUser [id = " + user.Id + "], requested by " + User.Identity.Name + ": " + e.Message);
            }

            return View(user);
        }

        #endregion

        #region Helpers

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private string CreateTempPassword(int length)
        {
            if (length < 1) length = 1;

            // N format is GUID with only digits (no hyphens)
            return Guid.NewGuid().ToString("N").Substring(0, length);
        }

        private string GetReturnUrl()
        {
            string url = Request.Url.GetLeftPart(UriPartial.Path);
            string returnUrl = Request.QueryString["ReturnUrl"];

            //if (!string.IsNullOrEmpty(returnUrl) && url.EndsWith(returnUrl)) return null;

            if (string.IsNullOrEmpty(returnUrl) && Request.UrlReferrer != null)
            {
                string queryStr = HttpUtility.UrlDecode(Request.UrlReferrer.Query);
                var queries = queryStr.Split(new char[] { '?', '&' });

                foreach (var item in queries)
                {
                    if (item.StartsWith("ReturnUrl"))
                    {
                        var query = item.Split(new char[] { '=' });
                        if (query.Length > 1)
                        {
                            string referrer = query[1];
                            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
                            returnUrl = baseUrl + referrer;
                            break;
                        }
                    }
                }
            }

            return returnUrl;
        }

        #endregion
    }
}
