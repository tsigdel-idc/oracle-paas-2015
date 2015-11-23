using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IDC.LeadCapture.Models.Assessment
{
    public class Answer
    {
        // custom wire format:
        // [Question OrderNo]-[Answer OrderNo]
        public string Name { get; set; }    

        public int OrderNo { get; set; }
        public long QuestionItemId { get; set; }
        public long AnswerChoiceId { get; set; }
        public AnswerType Type { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
        public string DefaultValue { get; set; }
        public bool Encrypted { get; set; }

        // alternative answer (e.g. Other)
        public bool AlternativeAnswer { get; set; }    // true if this is an alternative answer (such as 'Other' textbox or 'None of the Above' checkbox)
        public long? AltQuestionItemId { get; set; }     // QuestionItemId of the actual alternative answer texbox replacing the 'Other' DDL choice, if selected

        // for use with Scoring Model
        public decimal? Score { get; set; }

        public bool Selected { get; set; }
    }
}