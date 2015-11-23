CREATE PROCEDURE [dbo].[usp_AddQuestion]
	@aName nvarchar(50),				-- name of the main assessment
	@taName nvarchar(50) = NULL,		-- name of the translated assessment (optional)

	@qNo int,
	@qText nvarchar(4000),
	@qAltText nvarchar(4000) = NULL,	-- placeholder text in DDL such as 'Please select' (optional)
	@qValText nvarchar(4000) = NULL,	-- validation text (optional)

	@sectionNo int,						-- section number
	@sectionText nvarchar(4000) = NULL,	-- text to appear as section name or description (optional)

	@answerTypeName nvarchar(50) = NULL,-- if omitted, Question Type is assumed, or Checkbox if question is new
	@acNo int,							-- answer choice order
	@aacNo int,							-- can be used to change order
	@acName nvarchar(200) = NULL,		-- Address, Email, Country, Industry, Other, None of the above etc. (optional)
	@aacText nvarchar(4000) = NULL,		-- dropdown: Argentina, Austria, etc., all other types: answer choice text, Other, None of the above etc.  (optional)
	@link nvarchar(40) = NULL,			-- indicates a link to an alternative answer choice in DDL such as Other  (optional)
	@isAltChoice nvarchar(40) = NULL,	-- indicates that this is an alternative answer choice such as Other, None of the above ect. (optional)
	@score decimal(10, 5),

	@pageNo int = NULL					-- page number  (optional)
AS
BEGIN

	set nocount on

	--*****************************************************************************
	DECLARE @qName nvarchar(50)
	DECLARE @qId bigint

	DECLARE @aqId bigint
	DECLARE @aqName nvarchar(50)
	DECLARE @qrId bigint
	DECLARE @qrAltId bigint
	DECLARE @qrValId bigint

	DECLARE @aId bigint
	DECLARE @taId bigint
	DECLARE @answerTypeId int

	DECLARE @sectionName nvarchar(50)
	DECLARE @sectionId bigint
	DECLARE @srId bigint

	DECLARE @qiId bigint
	DECLARE @qiName nvarchar(50)
	DECLARE @qiDescription nvarchar(200)

	DECLARE @acId bigint

	DECLARE @aacName nvarchar(50)
	DECLARE @aacrId bigint
	DECLARE @aacId bigint
	DECLARE @refAlt bit = NULL	-- if set to 0, this is used as a placeholder for qiId of the alternative answer choice
	DECLARE @alt bit = 0		-- indicates that is an alternative answer choice (such as Other, None of the above ect.)

	DECLARE @resourceName nvarchar(200) 
	DECLARE @culture nvarchar(50) = NULL
	DECLARE @resourceType int

	--*****************************************************************************

	IF ISNULL(@culture, '') = '' SET @culture = 'en-US'
	SET @sectionId = NULL

	IF ISNULL(@isAltChoice, '') <> '' SET @alt = 1
	IF ISNULL(@link, '') <> '' SET @refAlt = 0

	--*****************************************************************************
	-- Assessment
	select @aId = Id from Assessment where Name = @aName

	IF @aId IS NULL 
	BEGIN
		insert into Assessment (Name) values (@aName)
		SET @aId = IDENT_CURRENT('Assessment')
	END

	if @@error <> 0 return

	-- create translatable resource for main assessment name
	exec usp_SetResourceValue @aName, @aName

	IF ISNULL(@taName, '') = ''
	BEGIN
		SET @taId = @aId		
	END
	ELSE
	BEGIN
		select @taId = Id from Assessment where Name = @taName

		IF @taId IS NULL 
		BEGIN
			insert into Assessment (Name) values (@taName)
			SET @taId = IDENT_CURRENT('Assessment')
		END

		-- create translated resource for assessment name
		IF (@culture <> 'en-US') exec usp_SetResourceValue @aName, @taName, @culture
	END

	if @@error <> 0 return

	--*****************************************************************************
	-- Question (q)
	SET @qName =  'A' + CONVERT(nvarchar, @aId) + '-' + 'Q' + CONVERT(nvarchar, @qNo)

	select @qId = Id, @qrId = ResourceId, @answerTypeId = AnswerTypeId from Question where Name = @qName

	-- Answer Type
	IF NOT ISNULL(@answerTypeName, '') = '' select @answerTypeId = Id from AnswerType where Name = @answerTypeName
	IF @answerTypeId IS NULL select @answerTypeId = Id from AnswerType where Name = 'Checkbox'

	IF @qId IS NULL
	BEGIN
		-- Question resource (qr)
		SET @resourceName = @qName
		select @qrId = r.Id from [Resource] r join ResourceValue rv on rv.ResourceId = r.Id
		where Name = @resourceName and CultureName = @culture

		IF @qrId IS NULL
		BEGIN
			SET @resourceType = 2 -- Question		
			exec usp_SetResourceValue @resourceName, @qText, @culture, @resourceType
			select @qrId = Id from [Resource] where Name = @resourceName
		END

		insert into Question (Name, ResourceId, AnswerTypeId, [Description]) values (@qName, @qrId, @answerTypeId, @qText)
		SET @qId = IDENT_CURRENT('Question')
	END

	if @@error <> 0 return

	-- Assessment Question (aq)
	SET @aqName = 'A' + CONVERT(nvarchar, @taId) + '-' + 'Q' + CONVERT(nvarchar, @qNo)
	select @aqId = Id from AssessmentQuestion where OrderNo = @qNo and AssessmentId = @taId

	IF @aqId IS NULL 
	BEGIN
		-- Section
		IF @sectionNo > 0
		BEGIN
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
					SET @resourceType = 2 -- Question				
					exec usp_SetResourceValue @resourceName, @sectionText, @culture, @resourceType
					select @srId = Id from [Resource] where Name = @resourceName
				END

				insert into Section (Name, ResourceId) values (@sectionName, @srId)
				SET @sectionId = IDENT_CURRENT('Section')
			END
		END

		-- alternative question text (use question names of the original assessment)
		IF ISNULL(@qAltText, '') <> ''
		BEGIN
			SET @resourceName = @qName + '-altText'

			select @qrAltId = r.Id from [Resource] r join ResourceValue rv on rv.ResourceId = r.Id 
			where Name = @resourceName and CultureName = @culture

			IF @qrAltId IS NULL
			BEGIN
				SET @resourceType = 2 -- Question				
				exec usp_SetResourceValue @resourceName, @qAltText, @culture, @resourceType
				select @qrAltId = Id from [Resource] where Name = @resourceName
			END
		END	

		-- validation text
		IF ISNULL(@qValText, '') <> ''
		BEGIN
			SET @resourceName = @qName + '-valText'

			select @qrValId = r.Id from [Resource] r join ResourceValue rv on rv.ResourceId = r.Id 
			where Name = @resourceName and CultureName = @culture

			IF @qrValId IS NULL
			BEGIN
				SET @resourceType = 2 -- Question				
				exec usp_SetResourceValue @resourceName, @qValText, @culture, @resourceType
				select @qrAltId = Id from [Resource] where Name = @resourceName
			END
		END

		insert into AssessmentQuestion
		(AssessmentId, Name, OrderNo, TextId, AlttextId, ValidationTextId, AnswerTypeId, SectionId, PageNo) VALUES
		(@taId, @qName, @qNo, @qrId, @qrAltId, @qrValId, @answerTypeId, @sectionId, @pageNo)

		select @aqId = IDENT_CURRENT('AssessmentQuestion')
	END

	if @@error <> 0 return

	--*****************************************************************************
	-- QuestionItem (qi) -> Answer Choice (ac)
	-- Checkbox:  1 to 1
	-- Radio:  1 to many
	-- DropDown: 1 to many
	-- Text: 1 to 1
	-- Integer: 1 to 1

	IF @answerTypeName = 'Checkbox'
	BEGIN
		-- allow multiple choice
		SET @qiName = @qName + '-' + CONVERT(nvarchar, @acNo)
	END
	ELSE
	BEGIN
		-- allow only 1 alternative answer choice
		IF ISNULL(@alt, 0) = 1 
		BEGIN
			SET @qiName = @qName + '-2'
			SET @acNo = 1
			SET @qiDescription = @acName
		END
		ELSE
		BEGIN
			SET @qiName = @qName + '-1'
			SET @qiDescription = @qText
		END
	END

	select @qiId = Id from QuestionItem where Name = @qiName

	IF @qiId IS NULL
	BEGIN
		INSERT INTO QuestionItem (QuestionId, AnswerTypeId, Name, [Description]) VALUES
		(@qId, @answerTypeId, @qiName, @qiDescription)

		SET @qiId = IDENT_CURRENT('QuestionItem')
	END

	-- link alternative answer choice to corresponding option in DDL
	IF ISNULL(@alt, 0) = 1
	BEGIN
		update AssessmentAnswerChoice set AltQuestionItemId = @qiId
		where AssessmentQuestionId = @aqId and AltQuestionItemId = 0
	END

	if @@error <> 0 return

	--*****************************************************************************
	-- AnswerChoice (ac) & AssesmenAnswerChoice (aac)
	IF @answerTypeName = 'Checkbox' SET @aacName = @qiName + '-1'
	ELSE SET @aacName = @qiName + '-' + CONVERT(nvarchar, @acNo)
		
	IF ISNULL(@acName, '') = '' SET @acName = @aacName

	select @acId = Id from AnswerChoice where QuestionItemId = @qiId and Name = @acName

	if @acId IS NULL
	BEGIN
		insert into AnswerChoice (Name, QuestionItemId) VALUES (@acName, @qiId)
		SET @acId = IDENT_CURRENT('AnswerChoice')
	END

	if @@error <> 0 return

	-- AssessmentAnswerChoice
	IF @aacNo < 1 SET @aacNo = @acNo

	select @aacId = Id from AssessmentAnswerChoice 
	where AssessmentQuestionId = @aqId and QuestionItemId = @qiId and AnswerChoiceId = @acId

	IF @aacId IS NULL
	BEGIN
		-- answer resource
		IF @answerTypeName = 'Dropdown' SET @resourceName = @acName
		ELSE SET @resourceName = @aacName

		select @aacrId = r.Id from [Resource] r join ResourceValue rv on rv.ResourceId = r.Id 
		where Name = @resourceName and CultureName = @culture

		IF @aacrId IS NULL
		BEGIN
			SET @resourceType = 3 -- QuestionItem
			IF ISNULL(@aacText, '') = '' SET @aacText = @acName
					
			exec usp_SetResourceValue @resourceName, @aacText, @culture, @resourceType
			select @aacrId = Id from [Resource] where Name = @resourceName 
		END

		-- set AltQuestionItemId = 1 so the record can be found later and linked to alternative answer choice
		insert into AssessmentAnswerChoice
		(AssessmentQuestionId, Name, OrderNo, TextId, AnswerTypeId, QuestionItemId, AnswerChoiceId, AltQuestionItemId, AlternativeAnswer, Score) VALUES
		(@aqId, @aacName, @aacNo, @aacrId, @answerTypeId, @qiId, @acId, @refAlt, @alt, @score)
	END

	print @aacName
END