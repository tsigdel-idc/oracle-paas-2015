using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IDC.Common;
using IDC.LeadCapture.Repository;
using IDC.LeadCapture.Models;
using IDC.LeadCapture.Models.Assessment;

namespace IDC.LeadCapture.DAL
{
    public class ReportsRepo
    {
        private ICryptoService _aes = new CryptoService();
        private const string dateFormat = "yyyy/MMM";

        public List<LeadCapture.Models.Response> GetResponses(DateTime startDate, DateTime endDate)
        {
            using (var ctx = new AssessmentEntities())
            {
                var query =
                    from response in ctx.Response
                    where !response.Deleted && response.DateCreated >= startDate && response.DateCreated < endDate
                    select new LeadCapture.Models.Response()
                    {
                        Id = response.Id,
                        DateCreated = response.DateCreated,
                        Email = response.Email,
                        CampaignId = response.CampaignId.HasValue ? (long)response.CampaignId : 0,
                        CompletedPageNo = response.CompletedPageNo.HasValue ? (int)response.CompletedPageNo : 0,
                        Completed = response.Completed,
                        ReportSent = response.ReportSent
                    };

                return query.ToList();
            }
        }

        public int GetActiveCampaignsCount()
        {
            using (var ctx = new AssessmentEntities())
            {
                 return ctx.Campaign.Where(x => !x.Disabled).Count();
            }
        }

        public int GetDisabledCampaignsCount()
        {
            using (var ctx = new AssessmentEntities())
            {
                return ctx.Campaign.Where(x => x.Disabled).Count();
            }
        }

        #region assessments list

        public List<Models.Assessment.Assessment> GetReport(string assessmentName, string reportViewName, string cultureName, DateTime startDate, DateTime endDate)
        {
            using (var ctx = new AssessmentEntities())
            {
                var query =
                    // Report View
                    from aView in ctx.Assessment
                    join aqView in ctx.AssessmentQuestion on aView.Id equals aqView.AssessmentId

                    // Question text
                    join res1 in ctx.Resource on aqView.TextId equals res1.Id into aqr1
                    from aqrText in aqr1.DefaultIfEmpty()
                    join resv1 in ctx.ResourceValue on aqrText.Id equals resv1.ResourceId into aqrv1
                    from aqrvText in aqrv1.Where(x => x.CultureName == cultureName).DefaultIfEmpty()

                    // Question
                    join aq in ctx.AssessmentQuestion on aqView.OrderNo equals aq.OrderNo
                    join a in ctx.Assessment on aq.AssessmentId equals a.Id

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

                    // Response
                    join response in ctx.Response on answer.ResponseId equals response.Id

                    // Overall score
                    join scr in ctx.Score on response.Id equals scr.ResponseId into scr1
                    from score in scr1.DefaultIfEmpty()

                    // Campaign
                    join camp in ctx.Campaign on response.CampaignId equals camp.Id into camp1
                    from campaign in camp1.DefaultIfEmpty()

                    where a.Name == assessmentName && aView.Name == reportViewName
                    && response.DateCreated >= startDate && response.DateCreated < endDate && response.Completed
                    && !aqView.Deleted && !aq.Deleted && !aac.Deleted

                    select new QuestionAnswer
                    {
                        // response
                        ResponseId = answer.ResponseId,
                        DateCreated = response.DateCreated,
                        ScoreOverall = score.Overall,

                        // question
                        QuestionNo = aq.OrderNo,
                        AssessmentQuestionId = aq.Id,
                        AnswerType = aq.AnswerTypeId,

                        PageNo = aq.PageNo,
                        QuestionText = aqrvText.Value,

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
                        Score = aac.Score,

                        // campaign
                        CampaignId = campaign != null ? campaign.Id : 0,
                        CampaignGuid = campaign != null ? campaign.Guid : Guid.Empty,
                        CampaignName = campaign != null ? campaign.Name : null,
                        CampaignDescription = campaign != null ? campaign.Description : null
                    };

                var list = MapAssessmentList(query.ToList());
                list.ForEach(x => x.Name = assessmentName);
                return list;
            }
        }

        List<Models.Assessment.Assessment> MapAssessmentList(List<QuestionAnswer> query)
        {
            var list = new List<Models.Assessment.Assessment>();

            Models.Assessment.Assessment model = null;
            Models.Assessment.Question q = null;
            Models.Assessment.Answer a = null;
            int totalPages = 0;

            long responseId = -1;
            int qNo = -1;
            int aNo = -1;

            bool next_r = false;
            bool next_q = false;
            bool next_a = false;

            foreach (var item in query.OrderBy(x => x.ResponseId).ThenBy(x => x.QuestionNo).ThenBy(x => x.AnswerNo))
            {
                next_r = item.ResponseId != responseId;
                next_q = item.QuestionNo != qNo || next_r;
                next_a = item.AnswerNo != aNo || next_q;

                responseId = item.ResponseId;
                qNo = item.QuestionNo;
                aNo = item.AnswerNo;

                if (next_r)
                {
                    model = new Models.Assessment.Assessment();
                    list.Add(model);

                    model.ResponseId = item.ResponseId;
                    model.DateCreated = item.DateCreated;
                    model.Score = item.ScoreOverall;

                    model.CampaignId = item.CampaignId;
                    model.CampaignGuid = item.CampaignGuid;
                    model.CampaignName = item.CampaignName;
                    model.CampaignDescription = item.CampaignDescription;

                    totalPages = 0;
                }

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

                    // to be used as HTML input element name
                    a.Name = string.Format("Q{0}-{1}", q.OrderNo, a.OrderNo);
                }

                if (item.PageNo.HasValue && item.PageNo > totalPages) totalPages = (int)item.PageNo;
                model.TotalPages = totalPages;
            }

            return list;
        }

        #endregion
    }
}