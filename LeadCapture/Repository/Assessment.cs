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
    
    public partial class Assessment
    {
        public Assessment()
        {
            this.AssessmentQuestion = new HashSet<AssessmentQuestion>();
        }
    
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Disabled { get; set; }
        public System.DateTime DateCreated { get; set; }
    
        public virtual ICollection<AssessmentQuestion> AssessmentQuestion { get; set; }
    }
}