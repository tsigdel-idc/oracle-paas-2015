using System;
using System.Collections.Generic;
using System.Linq;
using IDC.Common;
using IDC.LeadCapture.Models.Assessment;
using IDC.LeadCapture.DAL;

namespace IDC.LeadCapture.BLL
{
    public class ScoringModel
    {
        private const string _peerScore_Overall = "PeerScore_Overall";
        private const string _peerScore_Industry = "PeerScore_Industry";
        private const string _peerScore_OrgSize = "PeerScore_OrgSize";
        private const string _peerScore_Region = "PeerScore_Region";

        private const string _peerPct_Overall = "PeerPct_Overall";
        private const string _peerPct_Industry = "PeerPct_Industry";
        private const string _peerPct_OrgSize = "PeerPct_OrgSize";
        private const string _peerPct_Region = "PeerPct_Region";

        private const string _top10pct = "_Top10pct";

        private string[] _levelName = { "level1_name", "level2_name", "level3_name", "leve4_name" };
        private string[] _altLevelName = { "level1_alt_name", "level2_alt_name", "level3_alt_name", "leve4_alt_name" };



        public Models.Scoring GetReport(Assessment assessment)
        {
            var report = new Models.Scoring(assessment);
            report.ResponseId = assessment.ResponseId;
            report.ScoreOverall = GetOverallScore(assessment);

            // rounded score (level)
            int roundScore =  GetRoundScore(report.ScoreOverall);

            report.RoundScoreOverall = roundScore;
            report.LevelName = GetLevelName(roundScore);
            report.AltLevelName = GetAltLevelName(roundScore);

            // peer score of the top 10% of peers
            report.PeerScoreOverall_Top10Pct = Helper.DecimalValue(ResourceRepo.GetDefaultValue(_peerScore_Overall + _top10pct));           
            report.PeerScoreIndustry_Top10Pct = GetPeerScore(_peerScore_Industry + _top10pct, report.Industry);
            report.PeerScoreOrgSize_Top10Pct = GetPeerScore(_peerScore_OrgSize + _top10pct, report.OrgSize);
            report.PeerScoreRegion_Top10Pct = GetPeerScore(_peerScore_Region + _top10pct, report.Location);

            // percentage of peers at this level
            report.PeerPctOverall = Helper.IntValue(ResourceRepo.GetDefaultValue(_peerPct_Overall + "_Level" + roundScore));

            // difference in level between respondent and top 10% in given domain
            report.LevelDiffIndustry = GetLevelDiff(roundScore, report.PeerScoreIndustry_Top10Pct);
            report.LevelDiffOrgSize = GetLevelDiff(roundScore, report.PeerScoreOrgSize_Top10Pct);
            report.LevelDiffRegion = GetLevelDiff(roundScore, report.PeerScoreRegion_Top10Pct);

            report.Q2 = GetAnswers(assessment, 2);
            report.Q3 = GetAnswers(assessment, 3);
            report.Q5 = GetAnswers(assessment, 5);
            report.Q6 = GetAnswers(assessment, 6);
            report.Q8 = GetAnswers(assessment, 8);
            report.Q9 = GetAnswers(assessment, 9);
            report.Q10 = GetAnswers(assessment, 10);

            return report;
        }

        public int GetRoundScore(decimal score)
        {
            if (score <= 0) score = 1;
            return (int)Math.Ceiling(score);
        }

        public decimal GetPeerScore(string domain, string category)
        {
            return Helper.DecimalValue(ResourceRepo.GetResourceValue(domain, category));
        }

        public int GetPeerPct(string domain, string category)
        {
            return Helper.IntValue(ResourceRepo.GetResourceValue(domain, category));
        }

        public string GetLevelName(int score)
        {
            if (score < 1) score = 1;
            if (score > _levelName.Length) score = _levelName.Length;
            return ResourceCache.Localize(_levelName[score - 1]);
        }

        public string GetAltLevelName(int score)
        {
            if (score < 1) score = 1;
            if (score > _altLevelName.Length) score = _altLevelName.Length;
            return ResourceCache.Localize(_altLevelName[score - 1]);
        }

        private int GetLevelDiff(int level, decimal peerScore)
        {
            int peerLevel = GetRoundScore(peerScore);
            return level - peerLevel;
        }

        private bool[] GetAnswers(Assessment assessment, int orderNo)
        {
            bool[] answers = null;
            var q = assessment.Questions.FirstOrDefault(x => x.OrderNo == orderNo);

            // complete items list
            if (q == null) q = QuestionCache.GetQuestion(orderNo);
            else
            {
                var qiList = QuestionCache.GetQuestion(orderNo).Answers;
                foreach (var item in qiList)
                {
                    if (!q.Answers.Any(x => x.AnswerChoiceId == item.AnswerChoiceId)) { q.Answers.Add(item); }
                }
            }

            int count = 0;
            var list = q.Answers.Where(x => !x.AlternativeAnswer);
            if (list != null) count = list.Count();
            if (count > 0)
            {
                int i = 0;
                answers = new bool[count];
                foreach (var item in list.OrderBy(x => x.OrderNo)) answers[i++] = item.Selected;
            }

            return answers;
        }

        #region helper methods

        private decimal GetOverallScore(Assessment assessment)
        {
            decimal score = 0;
            decimal scaled_score = 0;

            foreach (var question in assessment.Questions)
            {
                foreach (var answer in question.Answers)
                {
                    if (answer.Selected && answer.Score.HasValue)
                    {
                        score += (decimal)answer.Score;
                    }
                }
            }

            // scale score (from 0 to 100) to stages (from 1 to 4)
            scaled_score = score / 25;

            return scaled_score;
        }

        private decimal? GetDomainScore(int qNo, Assessment assessment)
        {
            decimal? score = null;

            var question = assessment.Questions.FirstOrDefault(x => x.OrderNo == qNo);
            if (question != null)
            {
                var answer = question.Answers.FirstOrDefault();
                if (answer != null) score = answer.Score;
            }

            return score;
        }

        #endregion
    }
}