using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IDC.LeadCapture.Models
{
    public enum ResourceType
    {
        Undefined,
        UI,
        Question,
        QuestionItem,
        Report,
        Email,
        DDL,
        Section
    }

    public class Resource
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public ResourceType Type { get; set; }

        public string TypeName { get; set; }

        public string Tag { get; set; }

        public string CultureName { get; set; }
    }
}