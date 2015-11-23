--select * from [dbo].[AssessmentQuestion] where AnswerTypeId = 2
--select * from Resource where Id = 193

declare @qname varchar(50) = 'A1-Q1'
--declare @qname varchar(50) = 'A1-Q2'
--declare @qname varchar(50) = 'A1-Q4'
--declare @qname varchar(50) = 'A1-Q5'
--declare @qname varchar(50) = 'A1-Q7'
--declare @qname varchar(50) = 'A1-Q9'

declare @vtextname varchar(50)

set @vtextname = @qname + '_val'
exec usp_SetResourceValue @vtextname, N'data-fv-notempty="true"'
update AssessmentQuestion set ValidationTextId = (select Id from Resource where Name = @vtextname) where Name = @qname

************************************************
declare @vtextname varchar(50)
declare @qname nvarchar(50)

set @qname = 'A1-Q3'
--set @qname = 'A1-Q6'
--set @qname = 'A1-Q8'
--set @qname = 'A1-Q10'
--set @qname = 'A1-Q19'

set @vtextname = @qname + '_val'
exec usp_SetResourceValue @vtextname, N'data-fv-notempty="true"'
update AssessmentQuestion set ValidationTextId = (select Id from Resource where Name = @vtextname) where Name = @qname

************************************************

set @qname = 'A1-Q11'
--set @qname = 'A1-Q15'
--set @qname = 'A1-Q16'
--set @qname = 'A1-Q17'
--set @qname = 'A1-Q18'

set @vtextname = @qname + '_val'
exec usp_SetResourceValue @vtextname, N'data-fv-notempty="true"'
update AssessmentQuestion set ValidationTextId = (select Id from Resource where Name = @vtextname) where Name = @qname

************************************************

set @qname = 'A1-Q12'
--set @qname = 'A1-Q13'
--set @qname = 'A1-Q14'

set @vtextname = @qname + '_val'
exec usp_SetResourceValue @vtextname, N'data-fv-notempty="true"'
update AssessmentQuestion set ValidationTextId = (select Id from Resource where Name = @vtextname) where Name = @qname