using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Globalization;
using IDC.LeadCapture.Models.Assessment;

namespace IDC.LeadCapture.BLL
{
    public class QuestionCache
    {
        private static string _assessmentName;
        private Assessment _assessment;        
        private Dictionary<int, Answer> _answers;
        private Dictionary<int, Question> _questions;

        static QuestionCache()
        {
            var _db = new IDC.LeadCapture.DAL.AssessmentRepo();

            // find culture-specific assessment name
            string assessmentNameConfig = ConfigurationManager.AppSettings["AssessmentName"] ?? "LeadCapture";
            string assessmentName = ResourceCache.Localize(assessmentNameConfig);
            if (string.IsNullOrEmpty(assessmentName)) assessmentName = assessmentNameConfig;

            // store definition of questions and answer chioces in cache
            var assessment = _db.GetQuestions(assessmentName, CultureInfo.CurrentUICulture.TextInfo.CultureName);
            var questions = new QuestionCache(assessment);
            if (HttpRuntime.Cache.Get(assessmentName) as QuestionCache == null) HttpRuntime.Cache.Insert(assessmentName, questions);

            // identify special questions and answer choices
            string industrySelection_Banking = ConfigurationManager.AppSettings["IndustrySelection_Banking"] ?? "Banking";
            string orgSizeSelection_Less_than_100 = ConfigurationManager.AppSettings["OrgSizeSelection_Less_than_100"] ?? "Less than 100";
            string countrySelection_USA = ConfigurationManager.AppSettings["CountrySelection_USA"] ?? "United States";
            
            string emailAnswerChoice = ConfigurationManager.AppSettings["EmailAnswerChoice"] ?? "Email";
            string firstNameAnswerChoice = ConfigurationManager.AppSettings["FirstNameAnswerChoice"] ?? "First name";
            string lastNameAnswerChoice = ConfigurationManager.AppSettings["LastNameAnswerChoice"] ?? "Last name";
            string companyNameAnswerChoice = ConfigurationManager.AppSettings["CompanyNameAnswerChoice"] ?? "Company Name";

            long industryChoiceId_Banking = _db.GetAnswerChoiceId(industrySelection_Banking);
            long orgSizeChoiceId_Less_than_100 = _db.GetAnswerChoiceId(orgSizeSelection_Less_than_100);
            long countryChoiceId_USA = _db.GetAnswerChoiceId(countrySelection_USA);

            EmailChoiceId = _db.GetAnswerChoiceId(emailAnswerChoice);

            long firstNameChoiceId = _db.GetAnswerChoiceId(firstNameAnswerChoice);
            long lastNameChoiceId = _db.GetAnswerChoiceId(lastNameAnswerChoice);
            long companyNameChoiceId = _db.GetAnswerChoiceId(companyNameAnswerChoice);

            foreach (var question in questions.Questions)
            {
                foreach (var answer in question.Answers)
                {
                    if (answer.AnswerChoiceId == EmailChoiceId)
                    {
                        EmailEncrypted = answer.Encrypted;
                        EmailQuestionNo = question.OrderNo;
                    }
                    else if (answer.AnswerChoiceId == firstNameChoiceId)
                    {
                        FirstNameQuestionNo = question.OrderNo;
                    }
                    else if (answer.AnswerChoiceId == lastNameChoiceId)
                    {
                        LastNameQuestionNo = question.OrderNo;
                    }
                    else if (answer.AnswerChoiceId == companyNameChoiceId)
                    {
                        CompanyQuestionNo = question.OrderNo;
                    }
                    else if (answer.AnswerChoiceId == countryChoiceId_USA)
                    {
                        CountryQuestionNo = question.OrderNo;
                        break;
                    }
                    else if (answer.AnswerChoiceId == orgSizeChoiceId_Less_than_100)
                    {
                        OrgSizeQuestionNo = question.OrderNo;
                        break;
                    }
                    else if (answer.AnswerChoiceId == industryChoiceId_Banking)
                    {
                        IndustryQuestionNo = question.OrderNo;
                        break;
                    }
                }
            }
        }

        private QuestionCache(Assessment assessment)
        {
            _assessmentName = assessment.Name;
            _assessment = assessment;

            _questions = new Dictionary<int, Question>();
            _answers = new Dictionary<int, Answer>();

            foreach (var question in assessment.Questions)
            {
                if (!_questions.ContainsKey(question.OrderNo)) _questions.Add(question.OrderNo, question);

                foreach (var answer in question.Answers)
                {
                    int key = question.OrderNo * 1000 + answer.OrderNo;
                    if (!_answers.ContainsKey(key)) _answers.Add(key, answer);
                }                
            }
        }

        /// <summary>
        /// Get a Question
        /// </summary>
        public Question this[int key]
        {
            get
            {
                return _questions.ContainsKey(key) ? _questions[key] : new Question();
            }
        }

        /// <summary>
        /// Get a Question Item
        /// </summary>
        public Answer this[int questionNo, int answerNo]
        {
            get
            {
                int key = questionNo * 1000 + answerNo;
                return _answers.ContainsKey(key) ? _answers[key] : null;
            }
        }

        public Assessment Assessment
        {
            get { return _assessment; }
        }

        public List<Question> Questions
        {
            get { return _assessment.Questions; }
        }

        public static Assessment GetAssessment()
        {
            return (HttpRuntime.Cache.Get(_assessmentName) as QuestionCache).Assessment;
        }

        public static Assessment GetAssessment(int pageNo)
        {
            var assessment = new Assessment()
            {
                Name = _assessmentName,
                CurrentPageNo = pageNo,
                Questions = GetQuestions(pageNo)
            };

            return assessment;
        }

        public static List<Question> GetQuestions(int pageNo)
        {
            return GetAssessment().Questions.Where(x => x.PageNo == pageNo).ToList();
        }

        public static Question GetQuestion(int questionNo)
        {
            var survey = HttpRuntime.Cache.Get(_assessmentName) as QuestionCache;
            var question = survey[questionNo];
            return question;
        }

        public static Answer GetAnswerChoice(int questionNo, int answerNo)
        {
            var survey = HttpRuntime.Cache.Get(_assessmentName) as QuestionCache;
            var questionItem = survey[questionNo, answerNo];
            return questionItem;
        }

        // get DDL items
        public static List<Answer> GetDdlItems(int questionNo)
        {
            var survey = HttpRuntime.Cache.Get(_assessmentName) as QuestionCache;
            var question = survey[questionNo];
            var list = question.Answers.Where(x => x.Type == AnswerType.DropDown).ToList();
            return list;
        }

        public static string AssessmentName { get { return _assessmentName; } }

        #region special questions and answer choices
        public static int IndustryQuestionNo { get; set; }
        public static int OrgSizeQuestionNo { get; set; }
        public static int CountryQuestionNo { get; set; }

        public static long EmailChoiceId { get; set; }
        public static bool EmailEncrypted { get; set; }

        public static int EmailQuestionNo { get; set; }
        public static int FirstNameQuestionNo { get; set; }
        public static int LastNameQuestionNo { get; set; }
        public static int CompanyQuestionNo { get; set; }

        #endregion
    }
}