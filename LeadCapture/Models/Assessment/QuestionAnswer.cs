using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IDC.LeadCapture.Models.Assessment
{
    public class QuestionAnswer
    {
        // response
        public long ResponseId;
        public DateTime DateCreated;
        public decimal? ScoreOverall;

        // question
        public int QuestionNo;
        public long AssessmentQuestionId;
        public int AnswerType;
        public string Section;
        public int? PageNo;
        public string QuestionText;
        public string AltText;
        public string ValidationText;
        public bool Optional;
        public bool QuestionGroup;
        public long? QuestionGroupId;

        // answer choice
        public int AnswerNo;
        public long QuestionItemId;
        public long AnswerChoiceId;
        public int ChoiceType;
        public string AnswerText;
        public string Value;
        public string DefaultValue;
        public bool Encrypted;
        public bool AlternativeAnswer;
        public long? AltQuestionItemId;
        public decimal? Score;

        // campaign
        public long CampaignId;
        public Guid CampaignGuid;
        public string CampaignName;
        public string CampaignDescription;
    }
}