INSERT INTO QuestionItem (QuestionId, AnswerTypeId, Name, [Description]) VALUES
(3, 1, 'A1-Q3-2', NULL),
(3, 1, 'A1-Q3-3', NULL),
(3, 1, 'A1-Q3-4', NULL),
(3, 1, 'A1-Q3-5', NULL),
(3, 1, 'A1-Q3-6', NULL),
(3, 1, 'A1-Q3-7', NULL),
(3, 1, 'A1-Q3-8', NULL)

update AnswerChoice set Name = 'A1-Q6-1-11' where Name = 'A1-Q6-2-1'
update AssessmentAnswerChoice set Name = 'A1-Q6-1-11' where Name = 'A1-Q6-2-1'

update AnswerChoice set Name = 'A1-Q8-1-8' where Name = 'A1-Q8-2-1'
update AssessmentAnswerChoice set Name = 'A1-Q8-1-8' where Name = 'A1-Q8-2-1'

update AnswerChoice set Name = 'A1-Q10-1-10' where Name = 'A1-Q10-2-1'
update AssessmentAnswerChoice set Name = 'A1-Q10-1-10' where Name = 'A1-Q10-2-1'

INSERT INTO QuestionItem (QuestionId, AnswerTypeId, Name, [Description]) VALUES
(6, 1, 'A1-Q6-2', NULL),
(6, 1, 'A1-Q6-3', NULL),
(6, 1, 'A1-Q6-4', NULL),
(6, 1, 'A1-Q6-5', NULL),
(6, 1, 'A1-Q6-6', NULL),
(6, 1, 'A1-Q6-7', NULL),
(6, 1, 'A1-Q6-8', NULL),
(6, 1, 'A1-Q6-9', NULL),
(6, 1, 'A1-Q6-10', NULL),
(6, 1, 'A1-Q6-11', NULL)

INSERT INTO QuestionItem (QuestionId, AnswerTypeId, Name, [Description]) VALUES
(8, 1, 'A1-Q8-2', NULL),
(8, 1, 'A1-Q8-3', NULL),
(8, 1, 'A1-Q8-4', NULL),
(8, 1, 'A1-Q8-5', NULL),
(8, 1, 'A1-Q8-6', NULL),
(8, 1, 'A1-Q8-7', NULL),
(8, 1, 'A1-Q8-8', NULL)

INSERT INTO QuestionItem (QuestionId, AnswerTypeId, Name, [Description]) VALUES
(10, 1, 'A1-Q10-2', NULL),
(10, 1, 'A1-Q10-3', NULL),
(10, 1, 'A1-Q10-4', NULL),
(10, 1, 'A1-Q10-5', NULL),
(10, 1, 'A1-Q10-6', NULL),
(10, 1, 'A1-Q10-7', NULL),
(10, 1, 'A1-Q10-8', NULL),
(10, 1, 'A1-Q10-9', NULL),
(10, 1, 'A1-Q10-10', NULL)

update ac set ac.QuestionItemId = qi.Id from AnswerChoice ac join QuestionItem qi on qi.Name = Replace(ac.Name, '-1-', '-') where qi.AnswerTypeId = 1
update aac set aac.QuestionItemId = qi.Id from AssessmentAnswerChoice aac join QuestionItem qi on qi.Name = Replace(aac.Name, '-1-', '-') where qi.AnswerTypeId = 1

delete from [dbo].[QuestionItem] where Id not in (select QuestionItemId from AnswerChoice)

update aac set aac.QuestionItemId = ac.QuestionItemId from AssessmentAnswerChoice aac join 
AnswerChoice ac on ac.Name = aac.Name where AnswerTypeId = 1

