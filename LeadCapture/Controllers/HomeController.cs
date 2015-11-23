using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Globalization;
using IDC.Common;
using IDC.LeadCapture.DAL;
using IDC.LeadCapture.BLL;
using IDC.LeadCapture.Models.Assessment;

namespace IDC.LeadCapture.Controllers
{
    public class HomeController : Controller
    {
        private string _culture = CultureInfo.CurrentUICulture.TextInfo.CultureName;
        private AssessmentRepo _db = new AssessmentRepo();

        public ActionResult Index(string id)
        {
            // reset temp data
            TempData["FormSubmitted"] = null;
            Session["ResponseKey"] = id;

            // allow robots to index the home page
            //ViewBag.MetaRobotsDirective = "all";

            // show maintenance notice
            //ViewBag.Notice = Models.Home.MaintenanceNotice.GetNotice();
            
            return View();
        }

        [HttpGet]
        public ViewResult Questionnaire(string page)
        {
            var model = new Assessment();
            long responseId = 0;
            int pageNo = 1;
            if (!string.IsNullOrEmpty(page) && !int.TryParse(page, out pageNo)) pageNo = 1;

            if (Session["ResponseId"] != null) long.TryParse(Session["ResponseId"].ToString(), out responseId);
            string responseKey = Session["ResponseKey"] != null ? Session["ResponseKey"].ToString() : null;

            if (Session["TargetPage"] == null) Session["TargetPage"] = "1";

            if (responseId == 0)
            {
                if (pageNo > 1) return null;    // should not be possible

                // record new response
                responseId = _db.CreateResponse(responseKey);
                Session["ResponseId"] = responseId;
                model = QuestionCache.GetAssessment(pageNo);
                Logger.Log(LogLevel.Info, "Assessment requested: ResponseId = " + responseId);
            }
            else
            {
                // retrieve previously stored answers
                var answeredQuestions = _db.GetAnswers(QuestionCache.AssessmentName, _culture, responseId).Questions;
                var questions = QuestionCache.GetAssessment(pageNo).Questions;

                // complete questions list with choices not found in the saved answers
                foreach(var question in questions)
                {
                    var answeredQuestion = answeredQuestions.FirstOrDefault(x => x.Name == question.Name);
                    if (answeredQuestion != null) model.Questions.Add(answeredQuestion);
                    else model.Questions.Add(question);
                }

                model.CurrentPageNo = pageNo;
            }

            return View(model);
        }

        [HttpGet]
        public ViewResult OrgProfile()
        {
            var model = new Assessment();
            long responseId = 0;
            int pageNo = 2;

            if (Session["ResponseId"] != null) long.TryParse(Session["ResponseId"].ToString(), out responseId);
            string responseKey = Session["ResponseKey"] != null ? Session["ResponseKey"].ToString() : null;

            if (responseId == 0)
            {
                return null;
            }
            else
            {
                // retrieve previously stored answers
                var answeredQuestions = _db.GetAnswers(QuestionCache.AssessmentName, _culture, responseId).Questions;
                var questions = QuestionCache.GetAssessment(pageNo).Questions;

                // complete questions list with choices not found in the saved answers
                foreach (var question in questions)
                {
                    var answeredQuestion = answeredQuestions.FirstOrDefault(x => x.Name == question.Name);
                    if (answeredQuestion != null) model.Questions.Add(answeredQuestion);
                    else model.Questions.Add(question);
                }

                model.CurrentPageNo = pageNo;
            }

            return View(model);
        }


        [HttpGet]
        public ActionResult Result()
        {
            var model = TempData["Report"] as LeadCapture.Models.Scoring;

            if (model == null)
            {
                if (Session["ResponseId"] == null) return View();
                long responseId = 0;
                long.TryParse(Session["ResponseId"].ToString(), out responseId);
                var answers = _db.GetAnswers(QuestionCache.AssessmentName, _culture, responseId);
                var scoringModel = new ScoringModel();
                model = scoringModel.GetReport(answers);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Assessment(FormCollection form)
        {
            string result = "success";
            string targetUrl = "/";
            int targetPage = 1;
            long responseId = 0;
        
            if (Session["ResponseId"] != null) long.TryParse(Session["ResponseId"].ToString(), out responseId);
            if (Session["TargetPage"] != null) int.TryParse(Session["TargetPage"].ToString(), out targetPage);

            try
            {
                if (responseId > 0)
                {
                    Logger.Log(LogLevel.Trace, "Assessment request: ResponseId = " + responseId);
                    var model = ProcessFormData(form);
                    model.ResponseId = responseId;

                    _db.SaveAnswers(model);

                    if (model.CurrentPageNo > targetPage)
                    {
                        targetPage = model.CurrentPageNo;
                        Session["TargetPage"] = targetPage.ToString();
                    }

                    if (model.CurrentPageNo == 1)
                    {
                        targetUrl = Url.Action("OrgProfile", "Home");
                        if (targetPage < 2) targetPage = 2;
                        Session["TargetPage"] = targetPage.ToString();

                        Logger.Log(LogLevel.Trace, "Assessment processed [page = " + model.CurrentPageNo + "]: ResponseId = " + responseId);
                    }
                    else if (model.CurrentPageNo == 2)
                    {
                        model = _db.GetAnswers(QuestionCache.AssessmentName, _culture, responseId);
                        var scoringModel = new ScoringModel();
                        var report = scoringModel.GetReport(model);
                        _db.SaveScore(report);

                        targetUrl = Url.Action("Result", "Home");

                        if (targetPage < 3) targetPage = 3;
                        Session["TargetPage"] = targetPage.ToString();

                        TempData["Report"] = report;

                        Logger.Log(LogLevel.Trace, "Assessment processed [page = " + model.CurrentPageNo + "]: ResponseId = " + responseId);
                    }
                    else if (model.CurrentPageNo == 3)
                    {
                        _db.FinalizeResponse(model);
                        targetUrl = "#";    // not used on result page
                        Logger.Log(LogLevel.Info, "Assessment finished: ResponseId = " + responseId);

                        if (EmailDocument(responseId))
                        {
                            _db.FinalizeReport(model);
                            Session["ResponseId"] = 0;
                            Session["TargetPage"] = null;
                            Session.Abandon();
                        }
                        else
                        {
                            result = "error";
                            _db.SetErrorFlag(responseId);
                        }
                    }

                    // reset stale values in the UI
                    QuestionCache.GetQuestions(model.CurrentPageNo).ForEach(x => ModelState.Remove(x.Name));
                }
                else
                {
                    result = "error";
                    _db.SetErrorFlag(responseId);
                    Logger.Log(LogLevel.Error, "Request not processed: ResponseId = " + responseId);
                }
            }
            catch (Exception e)
            {
                result = "error";
                Logger.Log(LogLevel.Error, "Assessment processing error [ResponseId = " + responseId + "]: " + e.Message);
            }

            return Json(new
            {
                result = result,
                targetUrl = targetUrl,
                targetPage = targetPage.ToString()
            }, 
            JsonRequestBehavior.AllowGet);
        }

        #region form data

        // 1. read from the UI by answer Name or question Name
        // 2. match question definitiions stored in cashe by question No and answer No
        // 3. save in the db by question Id, question item Id and response Id
        private Assessment ProcessFormData(FormCollection form)
        {
            var model = new Assessment();

            // page number
            string name = "CurrentPage";
            string value = form[name];
            int pageNo = Helper.IntValue(value);
            model.CurrentPageNo = pageNo;

            // questions
            var questions = QuestionCache.GetQuestions(pageNo);

            // answers
            foreach(var question in questions)
            {
                var aq = new Models.Assessment.Question()
                {
                    Name = question.Name,
                    OrderNo = question.OrderNo,
                    AssessmentQuestionId = question.AssessmentQuestionId,
                    Type = question.Type,
                    PageNo = question.PageNo,
                };

                model.Questions.Add(aq);
                bool selected = false;

                foreach(var item in question.Answers)
                {
                    switch (item.Type)
                    {
                        case AnswerType.Integer:
                        case AnswerType.Text:
                            name = item.Name;       // item.Name
                            value = form[name];
                            selected = !string.IsNullOrEmpty(value);
                            break;
                        case AnswerType.DropDown:
                        case AnswerType.Radio:
                            name = question.Name;   // question.Name
                            value = form[name];
                            long choiceId = 0;
                            if (long.TryParse(value, out choiceId))
                            {
                                selected = choiceId == item.AnswerChoiceId;
                                value = null;
                            }
                            break;
                        case AnswerType.Checkbox:
                            name = item.Name;       // item.Name
                            value = form[name];

                            // false,false or true,false
                            var val = value.Split(',');
                            bool val0 = false;
                            bool val1 = false;

                            if (val.Length > 0)
                            {
                                bool.TryParse(val[0], out val0);
                                if (val.Length > 1) bool.TryParse(val[1], out val1);
                            }

                            selected = val0 || val1;
                            value = selected.ToString();
                            break;
                    }

                    // single choice questions: consider selected item
                    // multiple choice questions: consider all items
                    if (selected || item.Type == AnswerType.Checkbox)
                    {
                        var aac = new Models.Assessment.Answer()
                        {
                            Value = value,
                            Type = item.Type,
                            QuestionItemId = item.QuestionItemId,
                            AnswerChoiceId = item.AnswerChoiceId,
                            Name = item.Name,
                            Score = item.Score,
                            Selected = selected
                        };

                        aq.Answers.Add(aac);
                    }
                }
            }

            return model;
        }

        #endregion

        #region Helper methods

        private bool EmailDocument(long responseId)
        {
            var model = _db.GetAnswers(QuestionCache.AssessmentName, _culture, responseId);

            var scoringModel = new ScoringModel();
            var report = scoringModel.GetReport(model);

            var smtpMail = new SmtpMail();
            return smtpMail.SendReport(report);
        }

        #endregion
    }
}
