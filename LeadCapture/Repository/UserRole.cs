//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IDC.LeadCapture.Repository
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserRole
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int RoleId { get; set; }
        public bool Disabled { get; set; }
    
        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
    }
}