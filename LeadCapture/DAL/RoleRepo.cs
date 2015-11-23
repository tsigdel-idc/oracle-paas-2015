using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IDC.Common;
using IDC.LeadCapture.Repository;

namespace IDC.LeadCapture.DAL
{
    public class RoleRepo
    {
        private ICryptoService _aes = new CryptoService();

        public string[] GetAllRoles()
        {
            using (var ctx = new AssessmentEntities())
            {
                return ctx.Role.Select(x => x.Name).ToArray();
            }
        }

        public bool RoleExists(string roleName)
        {
            using (var ctx = new AssessmentEntities())
            {
                return ctx.Role.Where(x => x.Name == roleName).Any();
            }
        }

        public bool IsUserInRole(string username, string roleName)
        {
            string username_encrypted = _aes.Encrypt(username);

            using (var ctx = new AssessmentEntities())
            {
                return ctx.UserRole.Where(x => x.User.UserName == username_encrypted && x.Role.Name == roleName && !x.Disabled).Any();
            }
        }

        public string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            var list = new List<string>();
            string username_encrypted = _aes.Encrypt(usernameToMatch);
            var searchTerm = new Regex(username_encrypted);

            using (var ctx = new AssessmentEntities())
            {
                var list_encrypted = ctx.UserRole.Where(x => searchTerm.IsMatch(x.User.UserName) && x.Role.Name == roleName && !x.Disabled).Select(x => x.User);
                if (list_encrypted != null) foreach (var item in list_encrypted) list.Add(_aes.Decrypt(item.UserName));
            }

            return list.ToArray();
        }

        public string[] GetUsersInRole(string roleName)
        {
            var list = new List<string>();

            using (var ctx = new AssessmentEntities())
            {
                var list_encrypted = ctx.UserRole.Where(x => x.Role.Name == roleName && !x.Disabled).Select(x => x.User);
                if (list_encrypted != null) foreach (var item in list_encrypted) list.Add(_aes.Decrypt(item.UserName));
            }

            return list.ToArray();
        }

        public string[] GetRolesForUser(string username)
        {
            string username_encrypted = _aes.Encrypt(username != null ? username.Trim().ToLower() : username);

            using (var ctx = new AssessmentEntities())
            {
                var list = ctx.UserRole.Where(x => x.User.UserName == username_encrypted && !x.Disabled).Select(x => x.Role.Name);                
                if (list != null) return list.ToArray();
                else return new string[0];
            }
        }
    }
}