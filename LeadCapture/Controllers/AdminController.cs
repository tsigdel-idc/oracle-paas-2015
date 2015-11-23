using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Globalization;
using IDC.Common;
using IDC.LeadCapture.BLL;
using IDC.LeadCapture.DAL;
using IDC.LeadCapture.Models;

namespace IDC.LeadCapture.Controllers
{
    [Authorize(Roles = "Guest, Admin, Master")]
    public class AdminController : Controller
    {
        private string _culture = CultureInfo.CurrentUICulture.TextInfo.CultureName;
        private AdminRepo _db = new AdminRepo();

        //
        // GET: /Admin/

        [HttpGet]
        [Authorize(Roles = "Guest, Admin, Master")]
        public ActionResult Index()
        {
            return View();
        }

        #region Lead Sources

        [HttpGet]
        [Authorize(Roles = "Admin, Master")]
        public ActionResult ManageLeadSources(string includeDeleted)
        {
            bool flag = false;
            bool.TryParse(includeDeleted, out flag);
            ViewBag.IncludeDeleted = flag ? bool.TrueString : bool.FalseString;

            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
            var model = _db.GetCampaignList(baseUrl, flag);
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Master")]
        public ActionResult EditLeadSource(string id)
        {
            var model = GetCampaign(id);

            if (model == null)
            {
                ModelState.AddModelError("Error", ResourceCache.Localize("ui_record_not_found"));
                model = new Models.Admin.Campaign();
                Logger.Log(LogLevel.Error, "Error in EditLeadSource: LeadSource not found [id = " + id + "], requested by " + User.Identity.Name);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Master")]
        public ActionResult EditLeadSource(Models.Admin.Campaign model)
        {
            bool success = _db.SaveCampaign(model);

            if (success)
            {
                Logger.Log(LogLevel.Info, "LeadSource saved [id = " + model.Id + "], requested by " + User.Identity.Name);
                return RedirectToAction("ManageLeadSources");
            }
            else
            {
                ModelState.AddModelError("Error", ResourceCache.Localize("ui_changes_not_saved"));
                Logger.Log(LogLevel.Error, "Error in EditLeadSource: LeadSource not saved [" + model.Id + "], requested by " + User.Identity.Name);
                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Master")]
        public ActionResult DeleteLeadSource(string id)
        {
            var model = GetCampaign(id);

            if (model == null)
            {
                ModelState.AddModelError("Error", ResourceCache.Localize("ui_record_not_found"));
                model = new Models.Admin.Campaign();
                Logger.Log(LogLevel.Error, "Error in DedleteLeadSource: LeadSource not found [id = " + id + "], requested by " + User.Identity.Name);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Master")]
        public ActionResult DeleteLeadSource(Models.Admin.Campaign model)
        {
            model.Disabled = true;
            bool success = _db.ToggleCampaign(model);

            if (success)
            {
                Logger.Log(LogLevel.Info, "LeadSource deleted [id = " + model.Id + "], requested by " + User.Identity.Name);
                return RedirectToAction("ManageLeadSources");
            }
            else
            {
                ModelState.AddModelError("Error", ResourceCache.Localize("ui_changes_not_saved"));
                Logger.Log(LogLevel.Error, "Error in DedleteLeadSource: LeadSource not deleted [id = " + model.Id + "], requested by " + User.Identity.Name);
                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Master")]
        public ActionResult RestoreLeadSource(string id)
        {
            var model = GetCampaign(id);

            if (model == null)
            {
                ModelState.AddModelError("Error", ResourceCache.Localize("ui_record_not_found"));
                Logger.Log(LogLevel.Error, "Error in RestoreLeadSource: LeadSource not found [id = " + id + "], requested by " + User.Identity.Name);
                model = new Models.Admin.Campaign();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Master")]
        public ActionResult RestoreLeadSource(Models.Admin.Campaign model)
        {
            model.Disabled = false;
            bool success = _db.ToggleCampaign(model);

            if (success)
            {
                Logger.Log(LogLevel.Info, "LeadSource restored [id = " + model.Id + "], requested by " + User.Identity.Name);
                return RedirectToAction("ManageLeadSources");
            }
            else
            {
                ModelState.AddModelError("Error", ResourceCache.Localize("ui_changes_not_saved"));
                Logger.Log(LogLevel.Error, "Error in RestoreLeadSource: LeadSource not savede [id = " + model.Id + "], requested by " + User.Identity.Name);
                return View(model);
            }
        }

        #endregion

        #region Users

        [HttpGet]
        public ActionResult ManageUsers(string includeDeleted)
        {
            bool flag = false;
            bool.TryParse(includeDeleted, out flag);
            ViewBag.IncludeDeleted = flag ? bool.TrueString : bool.FalseString;

            var model = _db.GetUsers(User.Identity.Name, flag);
            return View(model);
        }        

        #endregion

        #region helpers

        private Models.Admin.Campaign GetCampaign(string id)
        {
            Models.Admin.Campaign campaign = null;
            long _id = 0;
            bool valid = string.IsNullOrEmpty(id) || long.TryParse(id, out _id);
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";

            if (valid)
            {
                if (_id == 0)
                {
                    campaign = new Models.Admin.Campaign();
                    campaign.Guid = Guid.NewGuid();
                    campaign.Link = baseUrl + campaign.Guid;
                }
                else
                {
                    campaign = _db.GetCampaign(_id, baseUrl);
                }
            }
            else
            {
                Logger.Log(LogLevel.Error, "Invalid Campaign Id: " + id + ", requested by " + User.Identity.Name);
            }

            return campaign;
        }

        #endregion
    }
}
