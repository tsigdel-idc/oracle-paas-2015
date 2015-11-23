using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IDC.LeadCapture.Models.Assessment
{
    public class Question
    {
        public Question()
        {
            Answers = new List<Answer>();
        }

        public string Name { get; set; }

        public int OrderNo { get; set; }
        public long AssessmentQuestionId { get; set; }
        public AnswerType Type { get; set; }
        public string Section { get; set; }
        public int? PageNo { get; set; }
        public string Text { get; set; }
        public string AltText { get; set; }
        public string ValidationText { get; set; }
        public bool Optional { get; set; }

        // question group definition (construct question tables)
        public bool QuestionGroup { get; set; }    // true if question group starts here
        public long? QuestionGroupId { get; set; }   // points to parent AssessmentQuestionId

        public List<Answer> Answers { get; set; }
        public Answer AltAnswer { get; set; }
    }
}