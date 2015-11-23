using System;
using System.Collections.Generic;
using System.Linq;
using IDC.Common;
using IDC.LeadCapture.Repository;
using IDC.LeadCapture.Models;

namespace IDC.LeadCapture.DAL
{
    public class AccountRepo
    {
        private ICryptoService _aes = new CryptoService();

        #region registration and authentication

        public bool Authenticate(string userName, string password, out Models.Account.UserStatus status)
        {
            status = Models.Account.UserStatus.Undefined;
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password)) return false;

            bool result = false;
            string userName_encrypted = _aes.Encrypt(userName.Trim().ToLowerInvariant());
            int status_suspended = (int)Models.Account.UserStatus.Suspended;

            using (var ctx = new AssessmentEntities())
            {
                try
                {
                    string password_hash = ctx.usp_GetHash(password).SingleOrDefault<string>();

                    var userObj = ctx.User.Where(x =>
                           x.UserName == userName_encrypted
                           && x.Password == password_hash
                           && x.StatusId != status_suspended
                           && (x.EndDate.HasValue ? (DateTime)x.EndDate : DateTime.MaxValue) > DateTime.Now
                           && !x.Deleted)
                           .SingleOrDefault();

                    if (userObj != null)
                    {
                        userObj.LastLoginDate = DateTime.Now;
                        status = (Models.Account.UserStatus)userObj.UserStatus.Id;
                        ctx.SaveChanges();
                        result = true;
                    }
                    else
                    {
                        Logger.Log(LogLevel.Info, "Login not allowed [user name = " + userName + "]");
                    }
                }
                catch (Exception e)
                {
                    Logger.Log(LogLevel.Error, "Error in Authenticate: " + e.Message);
                }
            }

            return result;
        }

        public bool ChangePassword(Models.Account.User user)
        {
            bool outcome = false;

            if (string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.NewPassword)
                || string.IsNullOrEmpty(user.NewPasswordConfirm)) return outcome;

            if (user.NewPassword != user.NewPasswordConfirm) return outcome;
            if (user.NewPassword == user.Password) return outcome;

            try
            {
                using (var ctx = new AssessmentEntities())
                {
                    string password_hash = ctx.usp_GetHash(user.NewPassword).SingleOrDefault<string>();

                    var userObj = ctx.User.Where(x => x.UserName == user.UserName).SingleOrDefault();
                    if (userObj != null)
                    {
                        userObj.Password = password_hash;
                        userObj.StatusId = (int)Models.Account.UserStatus.Active;
                        userObj.DateUpdated = DateTime.Now;
                        ctx.SaveChanges();
                        outcome = true;
                    }
                    else
                    {
                        Logger.Log(LogLevel.Error, "Error in ChangePassword [user name = " + user.UserName + "]: user not found");
                    }
                }
            }
            catch (Exception e)
            {
                outcome = false;
                Logger.Log(LogLevel.Error, "Error in ChangePassword: " + e.Message);
                throw new Exception();
            }

            return outcome;
        }

        public bool ResetPassword(Models.Account.User user)
        {
            bool outcome = false;

            if (string.IsNullOrEmpty(user.NewPassword)) return outcome;

            try
            {
                using (var ctx = new AssessmentEntities())
                {
                    string password_hash = ctx.usp_GetHash(user.NewPassword).SingleOrDefault<string>();

                    var userObj = ctx.User.Where(x => x.Email == user.Email).SingleOrDefault();
                    if (userObj != null)
                    {
                        // values used in 'reset password email'
                        user.UserName = userObj.UserName;
                        user.Guid = userObj.Guid;
                        user.FirstName = userObj.FirstName;
                        user.LastName = userObj.LastName;

                        // reset password
                        userObj.Password = password_hash;
                        userObj.StatusId = (int)Models.Account.UserStatus.Unconfirmed;
                        userObj.DateUpdated = DateTime.Now;
                        ctx.SaveChanges();
                        outcome = true;
                    }
                    else
                    {
                        Logger.Log(LogLevel.Error, "Error in ResetPassword [email = " + user.Email + "]: user not found");
                    }
                }
            }
            catch (Exception e)
            {
                outcome = false;
                Logger.Log(LogLevel.Error, "Error in ResetPassword: " + e.Message);
                throw new Exception();
            }

            return outcome;
        }

        #endregion

        #region user profile

        public Models.Account.User GetUser(long id)
        {
            var user = new Models.Account.User();

            try
            {
                using (var ctx = new AssessmentEntities())
                {
                    var userObj = ctx.User.FirstOrDefault(x => x.Id == id);

                    if (userObj != null)
                    {
                        var roles = new List<string>();
                        var roleObj = userObj.UserRole;
                        foreach (var item in roleObj.OrderBy(x => x.RoleId))
                        {
                            if (!item.Disabled)
                            {
                                var role = item.Role;
                                if (!role.Disabled) roles.Add(item.Role.Name);
                            }
                        }

                        user.Roles = roles;
                        user.Id = userObj.Id;
                        user.UserName = userObj.UserName;
                        user.FirstName = userObj.FirstName;
                        user.LastName = userObj.LastName;
                        user.CompanyName = userObj.CompanyName;
                        user.Email = userObj.Email;
                        if (userObj.EndDate.HasValue) user.EndDate = (DateTime)userObj.EndDate;
                        user.LastLoginDate = userObj.LastLoginDate;
                        user.Status = (Models.Account.UserStatus)userObj.StatusId;
                        user.Deleted = userObj.Deleted;
                    }
                    else
                    {
                        Logger.Log(LogLevel.Error, "Error in GetUser [user id = " + id + "]: user not found");
                    }
                }
            }
            catch (Exception e)
            {
                user.Status = Models.Account.UserStatus.Undefined;
                Logger.Log(LogLevel.Error, "Error in GetUser [user id = " + id + "]: " + e.Message);
            }

            return user;
        }

        public Models.Account.User GetUser(string userName)
        {
            var user = new Models.Account.User();

            try
            {
                using (var ctx = new AssessmentEntities())
                {
                    var userObj = ctx.User.FirstOrDefault(x => x.UserName == userName);

                    if (userObj != null)
                    {
                        var roles = new List<string>();
                        var roleObj = userObj.UserRole;
                        foreach(var item in roleObj.OrderBy(x => x.RoleId)) {
                            if (!item.Disabled)
                            {
                                var role = item.Role;
                                if (!role.Disabled) roles.Add(item.Role.Name);
                            }
                        }

                        user.Roles = roles;
                        user.Id = userObj.Id;
                        user.UserName = userObj.UserName;
                        user.FirstName = userObj.FirstName;
                        user.LastName = userObj.LastName;
                        user.CompanyName = userObj.CompanyName;
                        user.Email = userObj.Email;
                        if (userObj.EndDate.HasValue) user.EndDate = (DateTime)userObj.EndDate;
                        user.LastLoginDate = userObj.LastLoginDate;
                        user.Status = (Models.Account.UserStatus)userObj.StatusId;
                        user.Deleted = userObj.Deleted;
                    }
                    else
                    {
                        Logger.Log(LogLevel.Error, "Error in GetUser [user name = " + userName + "]: user not found");
                    }
                }
            }
            catch (Exception e)
            {
                user.Status = Models.Account.UserStatus.Undefined;
                Logger.Log(LogLevel.Error, "Error in GetUser [user name = " + userName + "]: " + e.Message);
            }

            return user;
        }

        public Models.Account.User GetUser(string key, string userName)
        {
            var user = new Models.Account.User();

            try
            {
                using (var ctx = new AssessmentEntities())
                {
                    var guid = Guid.Parse(key);
                    var userObj = ctx.User.FirstOrDefault(x => x.Guid == guid && x.UserName == userName);

                    if (userObj != null)
                    {
                        var roles = new List<string>();
                        var roleObj = userObj.UserRole;
                        foreach (var item in roleObj.OrderBy(x => x.RoleId))
                        {
                            if (!item.Disabled)
                            {
                                var role = item.Role;
                                if (!role.Disabled) roles.Add(item.Role.Name);
                            }
                        }

                        user.Roles = roles;
                        user.Id = userObj.Id;
                        user.UserName = userObj.UserName;
                        user.FirstName = userObj.FirstName;
                        user.LastName = userObj.LastName;
                        user.CompanyName = userObj.CompanyName;
                        user.Email = userObj.Email;
                        if (userObj.EndDate.HasValue) user.EndDate = (DateTime)userObj.EndDate;
                        user.LastLoginDate = userObj.LastLoginDate;
                        user.Status = (Models.Account.UserStatus)userObj.StatusId;
                        user.Deleted = userObj.Deleted;
                    }
                    else
                    {
                        Logger.Log(LogLevel.Error, "Error in GetUser [key = " + key + ", user name = " + userName + "]: user not found");
                    }
                }
            }
            catch (Exception e)
            {
                user.Status = Models.Account.UserStatus.Undefined;
                Logger.Log(LogLevel.Error, "Error in GetUser [key = " + key + ", user name = " + userName + "]: " + e.Message);
            }

            return user;
        }

        public bool AddUser(Models.Account.User user)
        {
            bool outcome = false;
            var now = DateTime.Now;

            if (user != null && string.IsNullOrEmpty(user.Email))
            {
                Logger.Log(LogLevel.Error, "Error in AddUser: email not provided");
                return outcome;
            }

            try
            {
                using (var ctx = new AssessmentEntities())
                {
                    if (ctx.User.Any(x => x.Email == user.Email || x.UserName == user.Email))
                    {
                        Logger.Log(LogLevel.Error, "Error in AddUser [email = " + user.Email + "]: user already exists");
                        return outcome;   // duplicate user
                    }

                    string password_hash = ctx.usp_GetHash(user.Password).SingleOrDefault<string>();

                    // add user
                    var userObj = new User()
                    {
                        Guid = user.Guid,
                        UserName = user.Email,
                        Password = password_hash,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        StatusId = (int)Models.Account.UserStatus.Unconfirmed,
                        StartDate = now,
                        DateCreated = now,
                        DateUpdated = now
                    };

                    ctx.User.Add(userObj);
                    ctx.SaveChanges();

                    // add Guest role
                    var roleGuestObj = ctx.Role.FirstOrDefault(x => x.Name.Contains("Guest"));

                    if (roleGuestObj != null)
                    {
                        var userRoleObj = new UserRole()
                        {
                            UserId = userObj.Id,
                            RoleId = roleGuestObj.Id
                        };

                        ctx.UserRole.Add(userRoleObj);
                    }

                    // add Admin role
                    if (user.IsAdmin)
                    {
                        var roleAdminObj = ctx.Role.FirstOrDefault(x => x.Name.Contains("Admin"));

                        if (roleAdminObj != null)
                        {
                            var userRoleObj = new UserRole()
                            {
                                UserId = userObj.Id,
                                RoleId = roleAdminObj.Id
                            };

                            ctx.UserRole.Add(userRoleObj);
                        }
                    }

                    ctx.SaveChanges();
                    outcome = true;
                }
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, "Error in AddUser" + (user != null ? " [email = " + user.Email + "]: " : ": ") + e.Message);
                throw new Exception();
            }

            return outcome;
        }

        public bool UpdateUser(Models.Account.User user, bool updateRole = false)
        {
            bool outcome = false;

            if (string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName)) return outcome;

            try
            {
                using (var ctx = new AssessmentEntities())
                {
                    var userObj = ctx.User.Where(x => x.Id == user.Id).SingleOrDefault();

                    if (userObj != null)
                    {
                        userObj.FirstName = user.FirstName;
                        userObj.LastName = user.LastName;
                        userObj.DateUpdated = DateTime.Now;

                        if (updateRole)
                        {
                            var userRoleObj = userObj.UserRole;
                            var roleAdminObj = ctx.Role.FirstOrDefault(x => x.Name.Contains("Admin"));
                            var roleMasterObj = ctx.Role.FirstOrDefault(x => x.Name.Contains("Master"));

                            if (roleAdminObj != null)
                            {
                                if (user.IsAdmin)
                                {
                                    if (!userRoleObj.Any(x => x.RoleId == roleAdminObj.Id))
                                    {
                                        var newUserRoleObj =
                                            new UserRole()
                                            {
                                                UserId = userObj.Id,
                                                RoleId = roleAdminObj.Id
                                            };

                                        ctx.UserRole.Add(newUserRoleObj);
                                    }
                                    else
                                    {
                                        foreach (var role in userRoleObj.Where(x => x.RoleId == roleAdminObj.Id)) role.Disabled = false;
                                    }
                                }
                                else
                                {
                                    foreach (var role in userRoleObj.Where(x => x.RoleId == roleAdminObj.Id)) role.Disabled = true;
                                }
                            }
                            else
                            {
                                foreach (var item in userRoleObj) item.Disabled = true;
                            }

                            if (roleMasterObj != null)
                            {
                                if (user.IsMaster)
                                {
                                    if (!userRoleObj.Any(x => x.RoleId == roleMasterObj.Id))
                                    {
                                        var newUserRoleObj =
                                            new UserRole()
                                            {
                                                UserId = userObj.Id,
                                                RoleId = roleMasterObj.Id
                                            };

                                        ctx.UserRole.Add(newUserRoleObj);
                                    }
                                    else
                                    {
                                        foreach (var role in userRoleObj.Where(x => x.RoleId == roleMasterObj.Id)) role.Disabled = false;
                                    }
                                }
                                else
                                {
                                    foreach (var role in userRoleObj.Where(x => x.RoleId == roleMasterObj.Id)) role.Disabled = true;
                                }
                            }
                            else
                            {
                                foreach (var item in userRoleObj) item.Disabled = true;
                            }
                        } // end updateRole

                        ctx.SaveChanges();
                        outcome = true;
                    }
                    else
                    {
                        Logger.Log(LogLevel.Error, "Error in  UpdateUser [user id = " + user.Id + "]: user not found");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, "Error in UpdateUser" + (user != null ? " [user id = " + user.Id + "]: " : ": ") + e.Message);
                throw new Exception();
            }

            return outcome;
        }

        public bool DeleteUser(Models.Account.User user, bool toggle = false)
        {
            bool outcome = false;

            using (var ctx = new AssessmentEntities())
            {
                try
                {
                    var userObj = ctx.User.FirstOrDefault(x => x.Id == user.Id && x.Deleted == toggle);

                    if (userObj != null)
                    {
                        userObj.Deleted = !toggle;
                        ctx.SaveChanges();
                        outcome = true;
                    }
                    else
                    {
                        Logger.Log(LogLevel.Error, "Error in  DeleteUser [user id = " + user.Id + "]: user not found");
                    }
                }
                catch (Exception e)
                {
                    Logger.Log(LogLevel.Error, "Error in DeleteUser" + (user != null ? " [user id = " + user.Id + "]: " : ": ") + e.Message);
                    throw new Exception();
                }
            }

            return outcome;
        }

        public bool SuspendUser(Models.Account.User user, bool toggle = false)
        {
            bool outcome = false;

            // toggle between active and suspended
            int status = toggle ? (int)Models.Account.UserStatus.Suspended : (int)Models.Account.UserStatus.Active;
            int newStatus = toggle ? (int)Models.Account.UserStatus.Active : (int)Models.Account.UserStatus.Suspended;

            using (var ctx = new AssessmentEntities())
            {
                try
                {
                    var userObj = ctx.User.FirstOrDefault(x => x.Id == user.Id && x.StatusId == status);

                    if (userObj != null)
                    {
                        userObj.StatusId = newStatus;
                        ctx.SaveChanges();
                        outcome = true;
                    }
                    else
                    {
                        Logger.Log(LogLevel.Error, "Error in  SuspendUser [user id = " + user.Id + "]: user not found");
                    }
                }
                catch (Exception e)
                {
                    Logger.Log(LogLevel.Error, "Error in SuspendUser" + (user != null ? " [user id = " + user.Id + "]: " : ": ") + e.Message);
                    throw new Exception();
                }
            }

            return outcome;
        }

        #endregion
    }
}