using System;
using System.Collections.Generic;
using System.Linq;
using IDC.Common;
using IDC.LeadCapture.Repository;
using IDC.LeadCapture.Models.Admin;

namespace IDC.LeadCapture.DAL
{
    public class AdminRepo
    {
        #region campaign

        public List<Models.Admin.Campaign> GetCampaignList(string baseUrl, bool includeDeleted)
        {
            List<Models.Admin.Campaign> list = null;
            string defaultLink = null;

            if (baseUrl != null)
            {
                defaultLink = baseUrl.TrimEnd('/');
                if (!baseUrl.EndsWith("/")) baseUrl += "/";
            }

            using (var ctx = new AssessmentEntities())
            {
                try
                {
                    var query = ctx.Campaign.AsQueryable();
                    if (!includeDeleted) query = query.Where(x => !x.Disabled);

                    var listObj = 
                        query.Select(campaign => new Models.Admin.Campaign
                        {
                            Id = campaign.Id,
                            Description = campaign.Description,
                            Guid = campaign.Guid,
                            Disabled = campaign.Disabled,
                            Status = campaign.Disabled ? CampaignStatus.Deleted : CampaignStatus.Active,
                            Link = campaign.Guid == Guid.Empty ? defaultLink : baseUrl + campaign.Guid
                        });

                    if (listObj != null)
                    {
                        list = listObj.ToList();
                    }
                    else
                    {
                        Logger.Log(LogLevel.Error, "Error in GetCampaignList: query result is null");
                    }
                }
                catch (Exception e)
                {
                    Logger.Log(LogLevel.Error, "Error in GetCampaignList: " + e.Message);
                }
            }

            if (list == null) list = new List<Models.Admin.Campaign>();

            return list;
        }

        public Models.Admin.Campaign GetCampaign(long id, string baseUrl)
        {
            var campaign = new Models.Admin.Campaign();
            string defaultLink = null;

            if (baseUrl != null)
            {
                if (baseUrl.EndsWith("/"))
                {
                    defaultLink = baseUrl.Remove(baseUrl.LastIndexOf("/"));
                }
                else
                {
                    defaultLink = baseUrl;
                    baseUrl += "/";
                }
            }

            using (var ctx = new AssessmentEntities())
            {
                try
                {
                    var campaignObj = ctx.Campaign.FirstOrDefault(x => x.Id == id);

                    if (campaignObj != null)
                    {
                        campaign = new Models.Admin.Campaign()
                        {
                            Id = campaignObj.Id,
                            Description = campaignObj.Description,
                            Guid = campaignObj.Guid,
                            Disabled = campaignObj.Disabled,
                            Status = campaignObj.Disabled ? CampaignStatus.Deleted : CampaignStatus.Active,
                            Link = campaignObj.Guid == Guid.Empty ? defaultLink : baseUrl + campaignObj.Guid.ToString()
                        };
                    }
                    else
                    {
                        Logger.Log(LogLevel.Error, "Error in GetCampaign: LeadSource not found [id = " + id + "]");
                    }
                }
                catch (Exception e)
                {
                    Logger.Log(LogLevel.Error, "Error in GetCampaign: " + e.Message);
                }
            }

            return campaign;
        }

        public bool SaveCampaign(Models.Admin.Campaign campaign)
        {
            bool success = false;
            long id = campaign.Id;

            if (string.IsNullOrEmpty(campaign.Description))
            {
                Logger.Log(LogLevel.Error, "Error in SaveCampaign: empty Description field");
                return success;
            }

            using (var ctx = new AssessmentEntities())
            {
                try
                {
                    string msg;
                    int name_maxlen = 128;
                    int desc_maxlen = 256;
                    int len = campaign.Description.Length;
                    string name = len > name_maxlen ? campaign.Description.Substring(0, name_maxlen) : campaign.Description;
                    string desc = len > desc_maxlen ? campaign.Description.Substring(0, desc_maxlen) : campaign.Description;

                    var campaignObj = ctx.Campaign.FirstOrDefault(x => x.Id == id);

                    if (campaignObj != null)
                    {
                        campaignObj.Name = name;
                        campaignObj.Description = desc;
                        msg = "LeadSource updated: [id = " + id + "], [" + desc + "]";
                    }
                    else
                    {
                        campaignObj = new Repository.Campaign();
                        campaignObj.Name = name;
                        campaignObj.Description = desc;
                        if (campaign.Guid != Guid.Empty) campaignObj.Guid = campaign.Guid;
                        ctx.Campaign.Add(campaignObj);
                        msg = "LeadSource created: [" + desc + "]";
                    }

                    ctx.SaveChanges();
                    success = true;
                    Logger.Log(LogLevel.Info, msg);
                }
                catch (Exception e)
                {
                    Logger.Log(LogLevel.Error, "Error in SaveCampaign: " + e.Message);
                }
            }

            return success;
        }

        public bool ToggleCampaign(Models.Admin.Campaign campaign)
        {
            bool success = false;
            long id = campaign.Id;

            using (var ctx = new AssessmentEntities())
            {
                try
                {
                    var campaignObj = ctx.Campaign.FirstOrDefault(x => x.Id == id);

                    if (campaignObj != null)
                    {
                        campaignObj.Disabled = campaign.Disabled;
                        ctx.SaveChanges();
                        success = true;
                    }
                    else
                    {
                        Logger.Log(LogLevel.Error, "Error in ToggleCampaign: LeadSource not found [id = " + id + "]");
                    }
                }
                catch (Exception e)
                {
                    Logger.Log(LogLevel.Error, "Error in ToggleCampaign: " + e.Message);
                }
            }

            return success;
        }

        #endregion

        #region users

        public List<Models.Account.User> GetUsers(string currentUserName, bool includeDeleted)
        {
            List<Models.Account.User> list = null;

            using (var ctx = new AssessmentEntities())
            {
                try
                {
                    var query = ctx.User.AsQueryable().Where(x => x.UserName != currentUserName);
                    if (!includeDeleted) query = query.Where(x => !x.Deleted);

                    var listObj =
                        query.Select(userObj => new Models.Account.User
                        {
                            Id = userObj.Id,
                            FirstName = userObj.FirstName,
                            LastName = userObj.LastName,
                            CompanyName = userObj.CompanyName,
                            Email = userObj.Email,
                            EndDate = userObj.EndDate.HasValue ? (DateTime)userObj.EndDate : DateTime.MaxValue,
                            LastLoginDate = userObj.LastLoginDate,
                            Status = (Models.Account.UserStatus)userObj.StatusId,
                            Roles = userObj.UserRole.OrderBy(x => x.RoleId).Where(x => !x.Disabled).Select(x => x.Role.Name).ToList(),
                            Deleted = userObj.Deleted
                        });

                    if (listObj != null)
                    {
                        list = listObj.ToList();
                    }
                }
                catch (Exception e)
                {
                    Logger.Log(LogLevel.Error, "Error in GetUsers: " + e.Message);
                }
            }

            if (list == null) list = new List<Models.Account.User>();
            return list;
        }       

        #endregion
    }
}