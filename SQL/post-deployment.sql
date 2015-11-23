INSERT [dbo].[ResourceType] ([Name]) VALUES 
(N'UI'),
(N'Question'),
(N'QuestionItem'),
(N'Report'),
(N'Email'),
(N'DDL'),
(N'Section')

INSERT [dbo].[AnswerType] ([Name], [MultipleChioce]) VALUES 
(N'Checkbox', 1),
(N'Radio', 0),
(N'DropDown', 0),
(N'Text', 0),
(N'Integer', 0)

INSERT [dbo].[Campaign] ([Guid], [Name], [Description]) VALUES (N'00000000-0000-0000-0000-000000000000', N'Default', N'Default')

insert into UserStatus (Name) values ('Unconfirmed'), ('Active'), ('Suspended')
insert into Role (Name) values ('Guest'), ('Admin'), ('Master')

--customtools@idc.com/Demo1234
--insert into dbo.[User] (UserName, [Password], Email, StatusId) 
--select 'customtools@idc.com', 'BAD3F4EC22A14B015FFB18E1D8671800', 'customtools@idc.com', Id from UserStatus where Name = 'Active'

--insert into UserRole (UserId, RoleId) 
--select u.Id, r.Id from dbo.[User] u join Role r on r.Name = 'Admin' where UserName = 'customtools@idc.com'
