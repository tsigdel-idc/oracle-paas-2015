using System;
using System.Collections.Generic;
using System.Linq;
using IDC.Common;
using IDC.LeadCapture.Models.Assessment;
using IDC.LeadCapture.BLL;
using IDC.LeadCapture.Repository;

namespace IDC.LeadCapture.DAL
{
    public class AssessmentRepo
    {
        private ICryptoService _aes = new CryptoService();

        #region Lead Capture

        public long CreateResponse(string responseKey)
        {
            long responseId = 0;
            var now = DateTime.Now;

            using (var ctx = new AssessmentEntities())
            {
                var response = new Response();
                response.DateCreated = now;
                response.DateUpdated = now;

                // save Respose Key and Campaign Id associated with this response
                Guid guid;
                if (!string.IsNullOrEmpty(responseKey) && Guid.TryParse(responseKey, out guid))
                {
                    response.ResponseKey = guid;

                    // check if this Response Key is associated with Campaign Id
                    var campaignObj = ctx.Campaign.FirstOrDefault(x => x.Guid.Equals(guid));
                    if (campaignObj != null) response.CampaignId = campaignObj.Id;

                    // check if this Response Key is associated with User Id
                    var userObj = ctx.User.FirstOrDefault(x => x.Guid.Equals(guid));
                    if (userObj != null)
                    {
                        response.UserId = userObj.Id;
                        if (userObj.PartnerId.HasValue) response.PartnerId = userObj.PartnerId;
                    }
                }

                var assessmentObj = ctx.Assessment.FirstOrDefault(x => x.Name == QuestionCache.AssessmentName);
                if (assessmentObj != null) response.AssessmentId = assessmentObj.Id;

                ctx.Response.Add(response);
                ctx.SaveChanges();
                responseId = response.Id;
            }

            return responseId;
        }

        public void FinalizeResponse(Models.Assessment.Assessment assessment)
        {
            long responseId = assessment.ResponseId;
            if (responseId <= 0) return;

            var now = DateTime.Now;
            string email = null;

            using (var ctx = new AssessmentEntities())
            {
                var emailAnswerObj = ctx.Answer.FirstOrDefault(x =>
                    x.ResponseId == responseId &&
                    x.AnswerChoiceId == QuestionCache.EmailChoiceId);

                if (emailAnswerObj != null) email = emailAnswerObj.Value;

                var responseObj = ctx.Response.FirstOrDefault(x => x.Id == responseId);
                if (responseObj != null)
                {
                    responseObj.Email = email;
                    responseObj.Completed = true;
                    responseObj.DateCompleted = now;
                }

                ctx.SaveChanges();
            }
        }

        public void FinalizeReport(Models.Assessment.Assessment assessment)
        {
            long responseId = assessment.ResponseId;
            if (responseId <= 0) return;

            var now = DateTime.Now;

            using (var ctx = new AssessmentEntities())
            {
                var responseObj = ctx.Response.FirstOrDefault(x => x.Id == responseId);
                if (responseObj != null)
                {
                    responseObj.ReportSent = true;
                    responseObj.DateReportSent = now;
                }

                ctx.SaveChanges();
            }
        }

        public void SetErrorFlag(long responseId)
        {
            using (var ctx = new AssessmentEntities())
            {
                var responseObj = ctx.Response.FirstOrDefault(x => x.Id == responseId);
                if (responseObj != null)
                {
                    responseObj.Error = true;
                    responseObj.DateUpdated = DateTime.Now;
                }

                ctx.SaveChanges();
            }
        }

        public void SaveScore(Models.Scoring report)
        {
            long responseId = report.ResponseId;
            if (responseId <= 0) return;

            var now = DateTime.Now;

            using (var ctx = new AssessmentEntities())
            {
                var scoreObj = ctx.Score.FirstOrDefault(x => x.ResponseId == responseId);

                if (scoreObj != null)
                {
                    scoreObj.Overall = report.ScoreOverall;
                    scoreObj.DateCreated = now;
                }
                else
                {
                    scoreObj = new Score()
                    {
                        ResponseId = responseId,
                        Overall = report.ScoreOverall,
                        DateCreated = now
                    };

                    ctx.Score.Add(scoreObj);
                }

                ctx.SaveChanges();
            }
        }

        public void GetScore(Models.Scoring report)
        {
            using (var ctx = new AssessmentEntities())
            {
                var scoreObj = ctx.Score.FirstOrDefault(x => x.ResponseId == report.ResponseId);
                if (scoreObj != null) 
                {
                    report.ScoreOverall = scoreObj.Overall;
                }
            }
        }

        public long GetAnswerChoiceId(string name)
        {
            long id = 0;

            using (var ctx = new AssessmentEntities())
            {
                var acObj = ctx.AnswerChoice.FirstOrDefault(x => x.Name == name);
                if (acObj != null) id = acObj.Id;
            }

            return id;
        }

        public string GetAnswerChoiceName(long id)
        {
            string name = null;

            using (var ctx = new AssessmentEntities())
            {
                var acObj = ctx.AnswerChoice.FirstOrDefault(x => x.Id == id);
                if (acObj != null) name = acObj.Name;
            }

            return name;
        }

        #endregion

        #region Questions

        public Models.Assessment.Assessment GetQuestions(string assessmentName, string cultureName)
        {
            using (var ctx = new AssessmentEntities())
            {
                var query =
                    from a in ctx.Assessment
                    // Question
                    join aq in ctx.AssessmentQuestion on a.Id equals aq.AssessmentId
                    // Question text
                    join res1 in ctx.Resource on aq.TextId equals res1.Id into aqr1
                    from aqrText in aqr1.DefaultIfEmpty()
                    join resv1 in ctx.ResourceValue on aqrText.Id equals resv1.ResourceId into aqrv1
                    from aqrvText in aqrv1.Where(x => x.CultureName == cultureName).DefaultIfEmpty()
                    // Alt text
                    join res2 in ctx.Resource on aq.AltTextId equals res2.Id into aqr2
                    from aqrAlt in aqr2.DefaultIfEmpty()
                    join resv2 in ctx.ResourceValue on aqrAlt.Id equals resv2.ResourceId into aqrv2
                    from aqrvAlt in aqrv2.Where(x => x.CultureName == cultureName).DefaultIfEmpty()
                    // Validation text
                    join res3 in ctx.Resource on aq.ValidationTextId equals res3.Id into aqr3
                    from aqrVld in aqr3.DefaultIfEmpty()
                    join resv3 in ctx.ResourceValue on aqrVld.Id equals resv3.ResourceId into aqrv3
                    from aqrvVld in aqrv3.Where(x => x.CultureName == cultureName).DefaultIfEmpty()

                    // Section
                    join sectionObj in ctx.Section on aq.SectionId equals sectionObj.Id into sec1
                    from sec in sec1.DefaultIfEmpty()
                    join res4 in ctx.Resource on sec.ResourceId equals res4.Id into aqr4
                    from secr in aqr4.DefaultIfEmpty()
                    join resv4 in ctx.ResourceValue on secr.Id equals resv4.ResourceId into aqrv4
                    from secrv in aqrv4.Where(x => x.CultureName == cultureName).DefaultIfEmpty()

                    // Answer choice
                    join aac in ctx.AssessmentAnswerChoice on aq.Id equals aac.AssessmentQuestionId
                    join res5 in ctx.Resource on aac.TextId equals res5.Id into aqr5
                    from aacr in aqr5.DefaultIfEmpty()
                    join resv5 in ctx.ResourceValue on aacr.Id equals resv5.ResourceId into aqrv5
                    from aacrv in aqrv5.Where(x => x.CultureName == cultureName).DefaultIfEmpty()

                    where a.Name == assessmentName && !aq.Deleted && !aac.Deleted

                    select new QuestionAnswer
                    {
                        // question
                        QuestionNo = aq.OrderNo,
                        AssessmentQuestionId = aq.Id,
                        AnswerType = aq.AnswerTypeId,
                        Section = secrv.Value,
                        PageNo = aq.PageNo,
                        QuestionText = aqrvText.Value,
                        AltText = aqrvAlt.Value,
                        ValidationText = aqrvVld.Value,               
                        QuestionGroup = aq.QuestionGroup,
                        QuestionGroupId = aq.QuestionGroupId,
                        Optional = aq.Optional,

                        // answer choice
                        AnswerNo = aac.OrderNo,
                        QuestionItemId = aac.QuestionItemId,
                        AnswerChoiceId = aac.AnswerChoiceId,
                        ChoiceType = aac.AnswerTypeId,
                        AnswerText = aacrv.Value,
                        Value = null,
                        DefaultValue = aacr.DefaultValue,
                        Encrypted = aac.Encrypted,
                        AlternativeAnswer = aac.AlternativeAnswer,
                        AltQuestionItemId = aac.AltQuestionItemId,
                        Score = aac.Score
                    };

                // map to view model
                var assessment = MapAssessment(query.ToList());

                foreach (var question in assessment.Questions)
                    foreach (var answer in question.Answers)
                        answer.Selected = false;

                assessment.Name = assessmentName;
                return assessment;
            }
        }

        public Models.Assessment.Assessment GetAnswers(string assessmentName, string cultureName, long responseId)
        {
            using (var ctx = new AssessmentEntities())
            {
                var query =
                    from a in ctx.Assessment
                    // Question
                    join aq in ctx.AssessmentQuestion on a.Id equals aq.AssessmentId
                    // Question text
                    join res1 in ctx.Resource on aq.TextId equals res1.Id into aqr1
                    from aqrText in aqr1.DefaultIfEmpty()
                    join resv1 in ctx.ResourceValue on aqrText.Id equals resv1.ResourceId into aqrv1
                    from aqrvText in aqrv1.Where(x => x.CultureName == cultureName).DefaultIfEmpty()
                    // Alt text
                    join res2 in ctx.Resource on aq.AltTextId equals res2.Id into aqr2
                    from aqrAlt in aqr2.DefaultIfEmpty()
                    join resv2 in ctx.ResourceValue on aqrAlt.Id equals resv2.ResourceId into aqrv2
                    from aqrvAlt in aqrv2.Where(x => x.CultureName == cultureName).DefaultIfEmpty()
                    // Validation text
                    join res3 in ctx.Resource on aq.ValidationTextId equals res3.Id into aqr3
                    from aqrVld in aqr3.DefaultIfEmpty()
                    join resv3 in ctx.ResourceValue on aqrVld.Id equals resv3.ResourceId into aqrv3
                    from aqrvVld in aqrv3.Where(x => x.CultureName == cultureName).DefaultIfEmpty()

                    // Section
                    join sectionObj in ctx.Section on aq.SectionId equals sectionObj.Id into sec1
                    from sec in sec1.DefaultIfEmpty()
                    join res4 in ctx.Resource on sec.ResourceId equals res4.Id into aqr4
                    from secr in aqr4.DefaultIfEmpty()
                    join resv4 in ctx.ResourceValue on secr.Id equals resv4.ResourceId into aqrv4
                    from secrv in aqrv4.Where(x => x.CultureName == cultureName).DefaultIfEmpty()

                    // Answer choice
                    join aac in ctx.AssessmentAnswerChoice on aq.Id equals aac.AssessmentQuestionId
                    join res5 in ctx.Resource on aac.TextId equals res5.Id into aqr5
                    from aacr in aqr5.DefaultIfEmpty()
                    join resv5 in ctx.ResourceValue on aacr.Id equals resv5.ResourceId into aqrv5
                    from aacrv in aqrv5.Where(x => x.CultureName == cultureName).DefaultIfEmpty()

                    // Answer
                    join answer in ctx.Answer 
                    on new {a1 = aac.AssessmentQuestionId, a2 = aac.QuestionItemId, a3 = aac.AnswerChoiceId}
                    equals new {a1 = answer.AssessmentQuestionId, a2 = answer.QuestionItemId, a3 = answer.AnswerChoiceId}

                    where a.Name == assessmentName && answer.ResponseId == responseId 
                        && !aq.Deleted && !aac.Deleted

                    select new QuestionAnswer
                    {
                        // question
                        QuestionNo = aq.OrderNo,
                        AssessmentQuestionId = aq.Id,
                        AnswerType = aq.AnswerTypeId,
                        Section = secrv.Value,
                        PageNo = aq.PageNo,
                        QuestionText = aqrvText.Value,
                        AltText = aqrvAlt.Value,
                        ValidationText = aqrvVld.Value,
                        QuestionGroup = aq.QuestionGroup,
                        QuestionGroupId = aq.QuestionGroupId,
                        Optional = aq.Optional,

                        // answer choice
                        AnswerNo = aac.OrderNo,
                        QuestionItemId = aac.QuestionItemId,
                        AnswerChoiceId = aac.AnswerChoiceId,
                        ChoiceType = aac.AnswerTypeId,
                        AnswerText = aacrv.Value,
                        Value = answer.Value,
                        DefaultValue = aacr.DefaultValue,
                        Encrypted = aac.Encrypted,
                        AlternativeAnswer = aac.AlternativeAnswer,
                        AltQuestionItemId = aac.AltQuestionItemId,
                        Score = aac.Score
                    };

                // map to view model
                var assessment = MapAssessment(query.ToList());
                assessment.Name = assessmentName;
                assessment.ResponseId = responseId;
                return assessment;
            }
        }

        public Models.Assessment.Assessment GetAnswers(string assessmentName, string cultureName, int pageNo, long responseId)
        {
            using (var ctx = new AssessmentEntities())
            {
                var query =
                    from a in ctx.Assessment
                    // Question
                    join aq in ctx.AssessmentQuestion on a.Id equals aq.AssessmentId
                    // Question text
                    join res1 in ctx.Resource on aq.TextId equals res1.Id into aqr1
                    from aqrText in aqr1.DefaultIfEmpty()
                    join resv1 in ctx.ResourceValue on aqrText.Id equals resv1.ResourceId into aqrv1
                    from aqrvText in aqrv1.Where(x => x.CultureName == cultureName).DefaultIfEmpty()
                    // Alt text
                    join res2 in ctx.Resource on aq.AltTextId equals res2.Id into aqr2
                    from aqrAlt in aqr2.DefaultIfEmpty()
                    join resv2 in ctx.ResourceValue on aqrAlt.Id equals resv2.ResourceId into aqrv2
                    from aqrvAlt in aqrv2.Where(x => x.CultureName == cultureName).DefaultIfEmpty()
                    // Validation text
                    join res3 in ctx.Resource on aq.ValidationTextId equals res3.Id into aqr3
                    from aqrVld in aqr3.DefaultIfEmpty()
                    join resv3 in ctx.ResourceValue on aqrVld.Id equals resv3.ResourceId into aqrv3
                    from aqrvVld in aqrv3.Where(x => x.CultureName == cultureName).DefaultIfEmpty()

                    // Section
                    join sectionObj in ctx.Section on aq.SectionId equals sectionObj.Id into sec1
                    from sec in sec1.DefaultIfEmpty()
                    join res4 in ctx.Resource on sec.ResourceId equals res4.Id into aqr4
                    from secr in aqr4.DefaultIfEmpty()
                    join resv4 in ctx.ResourceValue on secr.Id equals resv4.ResourceId into aqrv4
                    from secrv in aqrv4.Where(x => x.CultureName == cultureName).DefaultIfEmpty()

                    // Answer choice
                    join aac in ctx.AssessmentAnswerChoice on aq.Id equals aac.AssessmentQuestionId
                    join res5 in ctx.Resource on aac.TextId equals res5.Id into aqr5
                    from aacr in aqr5.DefaultIfEmpty()
                    join resv5 in ctx.ResourceValue on aacr.Id equals resv5.ResourceId into aqrv5
                    from aacrv in aqrv5.Where(x => x.CultureName == cultureName).DefaultIfEmpty()

                    // Answer
                    join answer in ctx.Answer
                    on new { a1 = aac.AssessmentQuestionId, a2 = aac.QuestionItemId, a3 = aac.AnswerChoiceId }
                    equals new { a1 = answer.AssessmentQuestionId, a2 = answer.QuestionItemId, a3 = answer.AnswerChoiceId }

                    where a.Name == assessmentName && aq.PageNo.HasValue && aq.PageNo == pageNo && answer.ResponseId == responseId
                    && !aq.Deleted && !aac.Deleted

                    select new QuestionAnswer
                    {
                        // question
                        QuestionNo = aq.OrderNo,
                        AssessmentQuestionId = aq.Id,
                        AnswerType = aq.AnswerTypeId,
                        Section = secrv.Value,
                        PageNo = aq.PageNo,
                        QuestionText = aqrvText.Value,
                        AltText = aqrvAlt.Value,
                        ValidationText = aqrvVld.Value,
                        QuestionGroup = aq.QuestionGroup,
                        QuestionGroupId = aq.QuestionGroupId,
                        Optional = aq.Optional,

                        // answer choice
                        AnswerNo = aac.OrderNo,
                        QuestionItemId = aac.QuestionItemId,
                        AnswerChoiceId = aac.AnswerChoiceId,
                        ChoiceType = aac.AnswerTypeId,
                        AnswerText = aacrv.Value,
                        Value = answer.Value,
                        DefaultValue = aacr.DefaultValue,
                        Encrypted = aac.Encrypted,
                        AlternativeAnswer = aac.AlternativeAnswer,
                        AltQuestionItemId = aac.AltQuestionItemId,
                        Score = aac.Score
                    };

                // map to view model
                var assessment = MapAssessment(query.ToList());
                assessment.Name = assessmentName;
                assessment.ResponseId = responseId;
                return assessment;
            }
        }

        public void SaveAnswers(Models.Assessment.Assessment assessment)
        {
            long responseId = assessment.ResponseId;
            if (responseId <= 0) return;

            var now = DateTime.Now;

            int completedPageNo = assessment.CurrentPageNo;
            int completedQuestionNo = 0;

            // save answers (value and answer choice Id) by question Id, question item Id and response Id
            using (var ctx = new AssessmentEntities())
            {
                foreach(var question in assessment.Questions)
                {
                    long aqId = question.AssessmentQuestionId;

                    foreach(var answer in question.Answers)
                    {
                        long qiId = answer.QuestionItemId;

                        if (question.PageNo.HasValue && question.PageNo > completedPageNo) completedPageNo = (int)question.PageNo;
                        if (question.OrderNo > completedQuestionNo) completedQuestionNo = question.OrderNo;

                        // answer choice/value
                        long aacId = answer.AnswerChoiceId;
                        string value = answer.Encrypted ? _aes.Encrypt(answer.Value) : answer.Value;

                        var answerObj = ctx.Answer.FirstOrDefault(x =>
                            x.AssessmentQuestionId == aqId &&
                            x.QuestionItemId == qiId &&
                            x.ResponseId == responseId);

                        if (answerObj != null)
                        {
                            answerObj.AnswerChoiceId = aacId;
                            answerObj.Value = value;
                        }
                        else
                        {
                            answerObj = new Repository.Answer()
                            {
                                AssessmentQuestionId = aqId,
                                QuestionItemId = qiId,
                                AnswerChoiceId = aacId,
                                Value = value,
                                ResponseId = responseId,
                                DateCreated = now
                            };

                            ctx.Answer.Add(answerObj);
                        }

                        // update assessment progress
                        var responseObj = ctx.Response.FirstOrDefault(x => x.Id == responseId);
                        if (responseObj != null)
                        {
                            if (responseObj.CompletedPageNo.HasValue && responseObj.CompletedPageNo > completedPageNo) completedPageNo = (int)responseObj.CompletedPageNo;
                            if (responseObj.CompletedQuestionNo.HasValue && responseObj.CompletedQuestionNo > completedQuestionNo) completedQuestionNo = (int)responseObj.CompletedQuestionNo;

                            if (completedPageNo > 0) responseObj.CompletedPageNo = completedPageNo;
                            if (completedQuestionNo > 0) responseObj.CompletedQuestionNo = completedQuestionNo;

                            responseObj.DateUpdated = now;
                        }

                        ctx.SaveChanges();
                    }
                }
            }
        }

        #endregion

        #region mapping

        Models.Assessment.Assessment MapAssessment(List<QuestionAnswer> query)
        {
            var model = new Models.Assessment.Assessment();
            Models.Assessment.Question q = null;
            Models.Assessment.Answer a = null;
            int totalPages = 0;

            int qNo = -1;
            int aNo = -1;

            bool next_q = false;
            bool next_a = false;

            foreach (var item in query.OrderBy(x => x.QuestionNo).ThenBy(x => x.AnswerNo))
            {
                next_q = item.QuestionNo != qNo;
                next_a = item.AnswerNo != aNo || next_q;

                qNo = item.QuestionNo;
                aNo = item.AnswerNo;

                if (next_q)
                {
                    q = new Models.Assessment.Question();
                    model.Questions.Add(q);

                    q.OrderNo = item.QuestionNo;
                    q.AssessmentQuestionId = item.AssessmentQuestionId;
                    q.Type = (Models.Assessment.AnswerType)item.AnswerType;
                    q.Section = item.Section;
                    q.PageNo = item.PageNo;
                    q.Text = item.QuestionText;
                    q.AltText = item.AltText;
                    q.ValidationText = item.ValidationText;
                    q.Optional = item.Optional;
                    q.QuestionGroup = item.QuestionGroup;
                    q.QuestionGroupId = item.QuestionGroupId;

                    q.Name = "Q" + q.OrderNo;
                }

                if (next_a)
                {
                    a = new Models.Assessment.Answer();
                    q.Answers.Add(a);
                    if (item.AlternativeAnswer) q.AltAnswer = a;

                    a.OrderNo = item.AnswerNo;
                    a.QuestionItemId = item.QuestionItemId;
                    a.AnswerChoiceId = item.AnswerChoiceId;
                    a.Type = (Models.Assessment.AnswerType)item.ChoiceType;
                    a.Text = item.AnswerText;
                    a.Value = item.Encrypted ? _aes.Decrypt(item.Value) : item.Value;
                    a.DefaultValue = item.Encrypted ? _aes.Decrypt(item.DefaultValue) : item.DefaultValue;
                    a.Encrypted = item.Encrypted;
                    a.AlternativeAnswer = item.AlternativeAnswer;
                    a.AltQuestionItemId = item.AltQuestionItemId;
                    a.Score = item.Score;

                    if (a.Type == Models.Assessment.AnswerType.Checkbox)
                    {
                        bool selected = false;
                        bool.TryParse(a.Value, out selected);
                        a.Selected = selected;
                    }
                    else
                    {
                        a.Selected = true;
                    }

                    // to be used as HTML input element name
                    a.Name = string.Format("Q{0}-{1}", q.OrderNo, a.OrderNo);
                }

                if (item.PageNo.HasValue && item.PageNo > totalPages) totalPages = (int)item.PageNo;
            }

            model.TotalPages = totalPages;

            return model;
        }

 

        #endregion
    }
}