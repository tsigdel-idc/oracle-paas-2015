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
    
    public partial class ResourceValue
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public string CultureName { get; set; }
        public long ResourceId { get; set; }
        public bool Deleted { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateUpdated { get; set; }
    
        public virtual Resource Resource { get; set; }
    }
}