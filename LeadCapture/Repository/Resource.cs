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
    
    public partial class Resource
    {
        public Resource()
        {
            this.AssessmentAnswerChoice = new HashSet<AssessmentAnswerChoice>();
            this.AssessmentQuestion = new HashSet<AssessmentQuestion>();
            this.AssessmentQuestion1 = new HashSet<AssessmentQuestion>();
            this.AssessmentQuestion2 = new HashSet<AssessmentQuestion>();
            this.Question = new HashSet<Question>();
            this.QuestionItem = new HashSet<QuestionItem>();
            this.ResourceValue = new HashSet<ResourceValue>();
            this.Section = new HashSet<Section>();
        }
    
        public long Id { get; set; }
        public string Name { get; set; }
        public string DefaultValue { get; set; }
        public Nullable<long> ResourceTypeId { get; set; }
        public string Tag { get; set; }
        public bool Deleted { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateUpdated { get; set; }
    
        public virtual ICollection<AssessmentAnswerChoice> AssessmentAnswerChoice { get; set; }
        public virtual ICollection<AssessmentQuestion> AssessmentQuestion { get; set; }
        public virtual ICollection<AssessmentQuestion> AssessmentQuestion1 { get; set; }
        public virtual ICollection<AssessmentQuestion> AssessmentQuestion2 { get; set; }
        public virtual ICollection<Question> Question { get; set; }
        public virtual ICollection<QuestionItem> QuestionItem { get; set; }
        public virtual ResourceType ResourceType { get; set; }
        public virtual ICollection<ResourceValue> ResourceValue { get; set; }
        public virtual ICollection<Section> Section { get; set; }
    }
}
