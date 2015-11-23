using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IDC.LeadCapture.BLL;
using IDC.LeadCapture.DAL;

namespace IDC.LeadCapture.Models
{
    public class Scoring
    {
        public Scoring() 
        { 
        }

        public Scoring(Models.Assessment.Assessment assessment)
        {
            ResponseId = assessment.ResponseId;
            FirstName = GetAnswerValue(assessment, QuestionCache.FirstNameQuestionNo);
            LastName = GetAnswerValue(assessment, QuestionCache.LastNameQuestionNo);
            CompanyName = GetAnswerValue(assessment, QuestionCache.CompanyQuestionNo);
            Email = GetAnswerValue(assessment, QuestionCache.EmailQuestionNo);

            var _repo = new AssessmentRepo();
            long choiceId = 0;

            choiceId = GetAnswerChoiceId(assessment, QuestionCache.IndustryQuestionNo);
            Industry = _repo.GetAnswerChoiceName(choiceId);

            choiceId = GetAnswerChoiceId(assessment, QuestionCache.OrgSizeQuestionNo);
            OrgSize = _repo.GetAnswerChoiceName(choiceId);

            choiceId = GetAnswerChoiceId(assessment, QuestionCache.CountryQuestionNo);
            Location = _repo.GetAnswerChoiceName(choiceId);
        }

        #region helper methods

        private string GetAnswerValue(Models.Assessment.Assessment assessment, int qNo)
        {
            string value = null;

            var question = assessment.Questions.FirstOrDefault(x => x.OrderNo == qNo);
            if (question != null)
            {
                var answer = question.Answers.FirstOrDefault();
                if (answer != null) value = answer.Value;
            }

            return value;
        }

        private long GetAnswerChoiceId(Models.Assessment.Assessment assessment, int qNo)
        {
            long id = 0;

            var question = assessment.Questions.FirstOrDefault(x => x.OrderNo == qNo);
            if (question != null)
            {
                var answer = question.Answers.FirstOrDefault();
                if (answer != null) id = answer.AnswerChoiceId;
            }

            return id;
        }

        #endregion

        #region properties

        public long ResponseId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }

        public string Industry { get; set; }
        public string OrgSize { get; set; }
        public string Location { get; set; }

        public string LevelName { get; set; }
        public string AltLevelName { get; set; }

        public decimal ScoreOverall { get; set; }
        public int RoundScoreOverall { get; set; }

        // peer score
        public decimal PeerScoreOverall_Top10Pct { get; set; }
        public decimal PeerScoreIndustry_Top10Pct { get; set; }
        public decimal PeerScoreOrgSize_Top10Pct { get; set; }
        public decimal PeerScoreRegion_Top10Pct { get; set; }

        // percentage of peers
        public int PeerPctOverall { get; set; }

        // difference in levels
        public int LevelDiffIndustry { get; set; }
        public int LevelDiffOrgSize { get; set; }
        public int LevelDiffRegion { get; set; }

        // key answers
        public bool[] Q2 { get; set; }
        public bool[] Q3 { get; set; }
        public bool[] Q5 { get; set; }
        public bool[] Q6 { get; set; }
        public bool[] Q8 { get; set; }
        public bool[] Q9 { get; set; }
        public bool[] Q10 { get; set; }

        #endregion
    }
}