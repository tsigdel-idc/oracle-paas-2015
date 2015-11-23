using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using IDC.LeadCapture.DAL;

namespace IDC.Extensions
{
    public class CustomRoleProvider : RoleProvider
    {
        private RoleRepo _db = new RoleRepo();

        public override string ApplicationName
        {
            get { return "CloudAdoption"; }
            set { throw new NotImplementedException(); }
        }

        public override bool RoleExists(string roleName)
        {
            return _db.RoleExists(roleName);
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            return _db.IsUserInRole(username, roleName);
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            return _db.FindUsersInRole(roleName, usernameToMatch);
        }

        public override string[] GetAllRoles()
        {
            return _db.GetAllRoles();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            return _db.GetUsersInRole(roleName);
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            return _db.GetRolesForUser(username);
        }
    }
}