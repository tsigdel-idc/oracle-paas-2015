select aac.Id, a.AnswerChoiceId, at.Name, a.Value, ac.Name, aac.Score, aq.OrderNo, aac.OrderNo
from Answer a
join [dbo].[AssessmentQuestion] aq on a.[AssessmentQuestionId] = aq.Id
join [dbo].[AnswerChoice] ac on a.[AnswerChoiceId] = ac.Id
join [dbo].[AssessmentAnswerChoice] aac on a.[AnswerChoiceId] = aac.[AnswerChoiceId] and aq.Id = aac.[AssessmentQuestionId]
join [dbo].[AnswerType] at on aac.AnswerTypeId = at.Id
where [ResponseId] = 67
order by aq.OrderNo, aac.OrderNo