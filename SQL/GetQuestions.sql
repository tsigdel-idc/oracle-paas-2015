select aac.QuestionItemId, aac.AltQuestionItemId, aac.AssessmentQuestionId, aq.Name aqName, aac.Name aacName, ac.Id acId, ac.Name acName, Score, aac.TextId, r.Name resName, aac.OrderNo, r.DefaultValue, rv.Value from AssessmentQuestion aq
join AssessmentAnswerChoice aac on aac.AssessmentQuestionId = aq.Id
join AnswerChoice ac on aac.AnswerChoiceId = ac.Id
join Resource r on aac.TextId = r.Id
join ResourceValue rv on r.Id = rv.ResourceId
where aq.OrderNo = 13
order by aq.OrderNo, aac.OrderNo


select
qat.Name AnswerType
,acat.Name ChoiceType

,aq.OrderNo QuestionNo
,aac.OrderNo AnswerNo

,q.Name QuestionDefinitionName
,aq.Name QuestionName
,qi.Name QuestionItemName
,ac.Name AnswerChoiceName
,aac.Name AssessmentAnswerName

,aq.Id AssessmentQuestionId
,aac.QuestionItemId
,aac.AnswerChoiceId
,aac.AlternativeAnswer
,aac.AltQuestionItemId

,secrv.Value Section

,aqrvText.Value QText
,aacrv.Value AnswerText

,aq.PageNo

from Assessment a

-- Question
join AssessmentQuestion aq on a.Id = aq.AssessmentId
join AnswerType qat on aq.AnswerTypeId = qat.Id

left join Resource aqrText on aq.TextId = aqrText.Id
left join ResourceValue aqrvText on aqrText.Id = aqrvText.ResourceId and aqrvText.CultureName = 'en-US'

left join Resource aqrAlt on aq.AltTextId = aqrAlt.Id
left join ResourceValue aqrvAlt on aqrAlt.Id = aqrvAlt.ResourceId and aqrvAlt.CultureName = 'en-US'

left join Resource aqrVld on aq.ValidationTextId = aqrVld.Id
left join ResourceValue aqrvVld on aqrVld.Id = aqrvVld.ResourceId and aqrvVld.CultureName = 'en-US'

-- Section
left join Section sec on aq.SectionId = sec.Id
left join Resource secr on sec.ResourceId = secr.Id 
left join ResourceValue secrv on secr.Id = secrv.ResourceId and secrv.CultureName = 'en-US'

-- Answer Choice
join AssessmentAnswerChoice aac on aq.Id = aac.AssessmentQuestionId
join AnswerChoice ac on aac.AnswerChoiceId = ac.Id
join QuestionItem qi on aac.QuestionItemId = qi.Id
join Question q on qi.QuestionId = q.Id
join AnswerType acat on aac.AnswerTypeId = acat.Id
left join Resource aacr on aac.TextId = aacr.Id
left join ResourceValue aacrv on aacr.Id = aacrv.ResourceId and aacrv.CultureName = 'en-US'

where a.Name = 'RicohIOI' 
order by aq.PageNo, aq.OrderNo, aac.OrderNo
