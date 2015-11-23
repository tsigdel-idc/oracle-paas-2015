DECLARE @aId bigint
DECLARE @aqId bigint
DECLARE @sectionNo int
DECLARE @sectionName nvarchar(50)
DECLARE @sectionId bigint
DECLARE @srId bigint
DECLARE @sectionText nvarchar(4000)

DECLARE @resourceName nvarchar(200) 
DECLARE @culture nvarchar(50) = 'en-US'
DECLARE @resourceType int

set @aId = 1
set @sectionNo = 5

SET @sectionName =  'A' + CONVERT(nvarchar, @aId) + '-' + 'Sec' + CONVERT(nvarchar, @sectionNo)
select @sectionId = Id from Section where Name = @sectionName

IF @sectionId IS NULL
BEGIN
	-- Section resource (sr)
	SET @resourceName = @sectionName

	select @srId = r.Id from [Resource] r join ResourceValue rv on rv.ResourceId = r.Id 
	where Name = @resourceName and CultureName = @culture

	IF @srId IS NULL
	BEGIN
		SET @sectionText = N'Organization Profile'
		SET @resourceType = 2 -- Question				
		exec usp_SetResourceValue @resourceName, @sectionText, @culture, @resourceType
		select @srId = Id from [Resource] where Name = @resourceName
	END

	insert into Section (Name, ResourceId) values (@sectionName, @srId)
	SET @sectionId = IDENT_CURRENT('Section')

	set @aqId = 11
	update AssessmentQuestion set SectionId = @sectionId where Id = @aqId
	set @aqId = 12
	update AssessmentQuestion set SectionId = @sectionId where Id = @aqId
	set @aqId = 13
	update AssessmentQuestion set SectionId = @sectionId where Id = @aqId
	set @aqId = 14
	update AssessmentQuestion set SectionId = @sectionId where Id = @aqId
END