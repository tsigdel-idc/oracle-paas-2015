using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IDC.LeadCapture.Models.Account
{
    public enum UserStatus
    {
        Undefined = 0,
        Unconfirmed = 1, 
        Active = 2,
        Suspended = 3,
        Expired = 100,
        Deleted = 110
    }

    public class User
    {
        private UserStatus _status;
        private DateTime? _explrationDate = DateTime.MaxValue;
        private bool _isAdmin;
        private bool _isMaster;

        public long Id { get; set; }
        public string UserName { get; set; }
        public Guid Guid { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName
        {
            get
            {
                return (!string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(LastName)) ? (LastName + ", " + FirstName) :
                    (string.IsNullOrEmpty(FirstName) ? LastName : FirstName);
            }
        }

        public string CompanyName { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirm { get; set; }

        public string AdminEmail { get; set; }
        public int? PartnerId { get; set; }

        public List<string> Roles { get; set; }

        public DateTime? LastLoginDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate
        {
            get { return _explrationDate.HasValue ? (DateTime)_explrationDate : DateTime.MaxValue; }
            set { _explrationDate = value; }
        }

        public UserStatus Status { 
            get 
            {
                return Deleted ? UserStatus.Deleted :
                    (Expired ? UserStatus.Expired : _status);
            }
            
            set
            {
                _status = value;
            }
        }

        public bool Deleted { get; set; }

        public bool Unconfirmed { get { return _status == UserStatus.Unconfirmed; } }
        public bool Active { get { return _status == UserStatus.Active; } }
        public bool Suspended { get { return _status == UserStatus.Suspended; } }

        public bool IsAdmin
        {
            get { return Roles != null ? Roles.Any(x => x.Contains("Admin")) : _isAdmin; }
            set { _isAdmin = value;  }
        }

        public bool IsMaster
        {
            get { return Roles != null ? Roles.Any(x => x.Contains("Master")) : _isMaster; }
            set { _isMaster = value; }
        }

        public bool Expired
        {
            get { return EndDate < DateTime.Now; }
        }

        public string AccountType
        {
            get
            {
                if (Roles == null || Roles.Count <= 0)
                {
                    return string.Empty;
                }
                else
                {
                    // return the highest role in hierarchy
                    // roles are assumed to be sorted by role id in ascending order
                    return Roles[Roles.Count - 1];
                }
            }
        }
    }
}