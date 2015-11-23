USE [master]
GO
/****** Object:  Database [943078_db_ricohioi_a]    Script Date: 11/5/2015 4:22:26 PM ******/
CREATE DATABASE [943078_db_ricohioi_a]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'943078_db_ricohioi_a', FILENAME = N'S:\dataroot\943078_db_ricohioi_a.mdf' , SIZE = 15360KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'943078_db_ricohioi_a_log', FILENAME = N'S:\dataroot\943078_db_ricohioi_a_log.ldf' , SIZE = 3840KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [943078_db_ricohioi_a] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [943078_db_ricohioi_a].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [943078_db_ricohioi_a] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET ARITHABORT OFF 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET  ENABLE_BROKER 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET  MULTI_USER 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [943078_db_ricohioi_a] SET DB_CHAINING OFF 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [943078_db_ricohioi_a] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'943078_db_ricohioi_a', N'ON'
GO
USE [943078_db_ricohioi_a]
GO
/****** Object:  Table [dbo].[Answer]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Answer](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[AssessmentQuestionId] [bigint] NOT NULL,
	[QuestionItemId] [bigint] NOT NULL,
	[AnswerChoiceId] [bigint] NOT NULL,
	[Value] [nvarchar](2000) NULL,
	[ResponseId] [bigint] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AnswerChoice]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnswerChoice](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[QuestionItemId] [bigint] NULL,
	[Name] [nvarchar](200) NULL,
	[Deleted] [bit] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AnswerType]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnswerType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[MultipleChioce] [bit] NOT NULL DEFAULT ((0)),
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_AnswerType_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Assessment]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Assessment](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](200) NULL,
	[Disabled] [bit] NOT NULL DEFAULT ((0)),
	[DateCreated] [datetime2](7) NOT NULL DEFAULT (getdate()),
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_Assessment_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AssessmentAnswerChoice]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssessmentAnswerChoice](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[AssessmentQuestionId] [bigint] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[OrderNo] [int] NOT NULL,
	[TextId] [bigint] NULL,
	[AnswerTypeId] [int] NOT NULL,
	[QuestionItemId] [bigint] NOT NULL,
	[AnswerChoiceId] [bigint] NOT NULL,
	[AltQuestionItemId] [bigint] NULL,
	[AlternativeAnswer] [bit] NOT NULL,
	[Encrypted] [bit] NOT NULL,
	[Score] [decimal](10, 5) NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[Deleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_AssessmentAnswerChoice_Order] UNIQUE NONCLUSTERED 
(
	[AssessmentQuestionId] ASC,
	[OrderNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AssessmentQuestion]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssessmentQuestion](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[AssessmentId] [bigint] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](200) NULL,
	[OrderNo] [int] NOT NULL,
	[TextId] [bigint] NULL,
	[AltTextId] [bigint] NULL,
	[ValidationTextId] [bigint] NULL,
	[AnswerTypeId] [int] NOT NULL,
	[SectionId] [bigint] NULL,
	[PageNo] [int] NULL,
	[QuestionGroup] [bit] NOT NULL,
	[QuestionGroupId] [bigint] NULL,
	[Deleted] [bit] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[Optional] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_AssessmentQuestion_Order] UNIQUE NONCLUSTERED 
(
	[AssessmentId] ASC,
	[OrderNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Campaign]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Campaign](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL DEFAULT (newid()),
	[Name] [nvarchar](128) NOT NULL,
	[Description] [nvarchar](256) NOT NULL,
	[Disabled] [bit] NOT NULL DEFAULT ((0)),
	[StartDate] [datetime2](7) NULL,
	[EndDate] [datetime2](7) NULL,
	[DateCreated] [datetime2](7) NOT NULL DEFAULT (getdate()),
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_Campaign_Guid] UNIQUE NONCLUSTERED 
(
	[Guid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_Campaign_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Log]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Log](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Level] [nvarchar](8) NULL,
	[Logger] [nvarchar](250) NULL,
	[Message] [nvarchar](4000) NULL,
	[Date] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Partner]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Partner](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[PartnerDetailsId] [int] NULL,
	[Disabled] [bit] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DateUpdated] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_Partner_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PartnerDetails]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PartnerDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Email] [nvarchar](256) NULL,
	[CCEmail] [nvarchar](256) NULL,
	[BCCEmail] [nvarchar](256) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_PartnerDetails_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Question]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Question](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ResourceId] [bigint] NULL,
	[AnswerTypeId] [int] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](200) NULL,
	[Deleted] [bit] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[QuestionItem]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuestionItem](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[QuestionId] [bigint] NULL,
	[ResourceId] [bigint] NULL,
	[AnswerTypeId] [int] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](200) NULL,
	[Deleted] [bit] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Resource]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Resource](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[DefaultValue] [nvarchar](4000) NULL,
	[ResourceTypeId] [bigint] NULL,
	[Tag] [nvarchar](100) NULL,
	[Deleted] [bit] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DateUpdated] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_Resource_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ResourceType]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResourceType](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](100) NULL,
	[Deleted] [bit] NOT NULL DEFAULT ((0)),
	[DateCreated] [datetime2](7) NOT NULL DEFAULT (getdate()),
	[DateUpdated] [datetime2](7) NOT NULL DEFAULT (getdate()),
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_ResourceType_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ResourceValue]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResourceValue](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Value] [nvarchar](4000) NOT NULL,
	[CultureName] [nvarchar](50) NOT NULL,
	[ResourceId] [bigint] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DateUpdated] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_ResourceValue] UNIQUE NONCLUSTERED 
(
	[ResourceId] ASC,
	[CultureName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Response]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Response](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ResponseKey] [uniqueidentifier] NULL,
	[UserId] [bigint] NULL,
	[PartnerId] [int] NULL,
	[CampaignId] [bigint] NULL,
	[Email] [nvarchar](128) NULL,
	[AssessmentId] [bigint] NULL,
	[CompletedPageNo] [int] NULL,
	[CompletedQuestionNo] [int] NULL,
	[Completed] [bit] NOT NULL,
	[ReportSent] [bit] NOT NULL,
	[Error] [bit] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DateUpdated] [datetime2](7) NOT NULL,
	[DateCompleted] [datetime2](7) NULL,
	[DateReportSent] [datetime2](7) NULL,
	[DateDeleted] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Role]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[Disabled] [bit] NOT NULL DEFAULT ((0)),
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_Role_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Score]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Score](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResponseId] [bigint] NOT NULL,
	[Overall] [decimal](10, 5) NOT NULL,
	[Domain1] [decimal](10, 5) NULL,
	[Domain2] [decimal](10, 5) NULL,
	[Domain3] [decimal](10, 5) NULL,
	[Domain4] [decimal](10, 5) NULL,
	[Domain5] [decimal](10, 5) NULL,
	[Domain6] [decimal](10, 5) NULL,
	[Domain7] [decimal](10, 5) NULL,
	[Domain8] [decimal](10, 5) NULL,
	[Domain9] [decimal](10, 5) NULL,
	[Domain10] [decimal](10, 5) NULL,
	[Deleted] [bit] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_Score_ResponseId] UNIQUE NONCLUSTERED 
(
	[ResponseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Section]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Section](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[ResourceId] [bigint] NULL,
	[Deleted] [bit] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_Section_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[User]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL DEFAULT (newid()),
	[UserName] [nvarchar](128) NOT NULL,
	[Password] [nvarchar](128) NOT NULL,
	[Email] [nvarchar](128) NOT NULL,
	[FirstName] [nvarchar](128) NULL,
	[LastName] [nvarchar](128) NULL,
	[CompanyName] [nvarchar](128) NULL,
	[PartnerId] [int] NULL,
	[IsPartner] [bigint] NOT NULL DEFAULT ((0)),
	[StatusId] [int] NOT NULL DEFAULT ((0)),
	[Deleted] [bit] NOT NULL DEFAULT ((0)),
	[StartDate] [datetime2](7) NOT NULL DEFAULT (getdate()),
	[EndDate] [datetime2](7) NULL,
	[LastLoginDate] [datetime2](7) NULL,
	[DateCreated] [datetime2](7) NOT NULL DEFAULT (getdate()),
	[DateUpdated] [datetime2](7) NOT NULL DEFAULT (getdate()),
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_User_Email] UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_User_UserName] UNIQUE NONCLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRole](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [bigint] NOT NULL,
	[RoleId] [int] NOT NULL,
	[Disabled] [bit] NOT NULL DEFAULT ((0)),
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_UserRole] UNIQUE NONCLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserStatus]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_UserStatus_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  View [dbo].[vLog]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vLog]
AS
SELECT TOP 1000 [Id]
      ,[Level]
      ,[Logger]
      ,[Message]
      ,dateadd(hh, -5, [Date]) [Date]
  FROM [Log]
GO
ALTER TABLE [dbo].[Answer] ADD  DEFAULT ((0)) FOR [Deleted]
GO
ALTER TABLE [dbo].[Answer] ADD  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[AnswerChoice] ADD  DEFAULT ((0)) FOR [Deleted]
GO
ALTER TABLE [dbo].[AnswerChoice] ADD  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[AssessmentAnswerChoice] ADD  DEFAULT ((0)) FOR [AlternativeAnswer]
GO
ALTER TABLE [dbo].[AssessmentAnswerChoice] ADD  DEFAULT ((0)) FOR [Encrypted]
GO
ALTER TABLE [dbo].[AssessmentAnswerChoice] ADD  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[AssessmentAnswerChoice] ADD  DEFAULT ((0)) FOR [Deleted]
GO
ALTER TABLE [dbo].[AssessmentQuestion] ADD  DEFAULT ((0)) FOR [QuestionGroup]
GO
ALTER TABLE [dbo].[AssessmentQuestion] ADD  DEFAULT ((0)) FOR [Deleted]
GO
ALTER TABLE [dbo].[AssessmentQuestion] ADD  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[AssessmentQuestion] ADD  DEFAULT ((0)) FOR [Optional]
GO
ALTER TABLE [dbo].[Partner] ADD  DEFAULT ((0)) FOR [Disabled]
GO
ALTER TABLE [dbo].[Partner] ADD  DEFAULT ((0)) FOR [Deleted]
GO
ALTER TABLE [dbo].[Partner] ADD  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[Partner] ADD  DEFAULT (getdate()) FOR [DateUpdated]
GO
ALTER TABLE [dbo].[Question] ADD  DEFAULT ((0)) FOR [Deleted]
GO
ALTER TABLE [dbo].[Question] ADD  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[QuestionItem] ADD  DEFAULT ((0)) FOR [Deleted]
GO
ALTER TABLE [dbo].[QuestionItem] ADD  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[Resource] ADD  DEFAULT ((0)) FOR [Deleted]
GO
ALTER TABLE [dbo].[Resource] ADD  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[Resource] ADD  DEFAULT (getdate()) FOR [DateUpdated]
GO
ALTER TABLE [dbo].[ResourceValue] ADD  DEFAULT ((0)) FOR [Deleted]
GO
ALTER TABLE [dbo].[ResourceValue] ADD  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[ResourceValue] ADD  DEFAULT (getdate()) FOR [DateUpdated]
GO
ALTER TABLE [dbo].[Response] ADD  DEFAULT ((0)) FOR [Completed]
GO
ALTER TABLE [dbo].[Response] ADD  DEFAULT ((0)) FOR [ReportSent]
GO
ALTER TABLE [dbo].[Response] ADD  DEFAULT ((0)) FOR [Error]
GO
ALTER TABLE [dbo].[Response] ADD  DEFAULT ((0)) FOR [Deleted]
GO
ALTER TABLE [dbo].[Response] ADD  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[Response] ADD  DEFAULT (getdate()) FOR [DateUpdated]
GO
ALTER TABLE [dbo].[Score] ADD  DEFAULT ((0)) FOR [Deleted]
GO
ALTER TABLE [dbo].[Score] ADD  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[Section] ADD  DEFAULT ((0)) FOR [Deleted]
GO
ALTER TABLE [dbo].[Section] ADD  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[AnswerChoice]  WITH CHECK ADD  CONSTRAINT [FK_AnswerChoice_QuestionItemId] FOREIGN KEY([QuestionItemId])
REFERENCES [dbo].[QuestionItem] ([Id])
GO
ALTER TABLE [dbo].[AnswerChoice] CHECK CONSTRAINT [FK_AnswerChoice_QuestionItemId]
GO
ALTER TABLE [dbo].[AssessmentAnswerChoice]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentAnswerChoice_AnswerChoiceId] FOREIGN KEY([AnswerChoiceId])
REFERENCES [dbo].[AnswerChoice] ([Id])
GO
ALTER TABLE [dbo].[AssessmentAnswerChoice] CHECK CONSTRAINT [FK_AssessmentAnswerChoice_AnswerChoiceId]
GO
ALTER TABLE [dbo].[AssessmentAnswerChoice]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentAnswerChoice_AnswerTypeId] FOREIGN KEY([AnswerTypeId])
REFERENCES [dbo].[AnswerType] ([Id])
GO
ALTER TABLE [dbo].[AssessmentAnswerChoice] CHECK CONSTRAINT [FK_AssessmentAnswerChoice_AnswerTypeId]
GO
ALTER TABLE [dbo].[AssessmentAnswerChoice]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentAnswerChoice_AssessmentQuestionId] FOREIGN KEY([AssessmentQuestionId])
REFERENCES [dbo].[AssessmentQuestion] ([Id])
GO
ALTER TABLE [dbo].[AssessmentAnswerChoice] CHECK CONSTRAINT [FK_AssessmentAnswerChoice_AssessmentQuestionId]
GO
ALTER TABLE [dbo].[AssessmentAnswerChoice]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentAnswerChoice_QuestionItemId] FOREIGN KEY([QuestionItemId])
REFERENCES [dbo].[QuestionItem] ([Id])
GO
ALTER TABLE [dbo].[AssessmentAnswerChoice] CHECK CONSTRAINT [FK_AssessmentAnswerChoice_QuestionItemId]
GO
ALTER TABLE [dbo].[AssessmentAnswerChoice]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentAnswerChoice_TextId] FOREIGN KEY([TextId])
REFERENCES [dbo].[Resource] ([Id])
GO
ALTER TABLE [dbo].[AssessmentAnswerChoice] CHECK CONSTRAINT [FK_AssessmentAnswerChoice_TextId]
GO
ALTER TABLE [dbo].[AssessmentQuestion]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentQuestion_AltTextId] FOREIGN KEY([AltTextId])
REFERENCES [dbo].[Resource] ([Id])
GO
ALTER TABLE [dbo].[AssessmentQuestion] CHECK CONSTRAINT [FK_AssessmentQuestion_AltTextId]
GO
ALTER TABLE [dbo].[AssessmentQuestion]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentQuestion_AssessmentId] FOREIGN KEY([AssessmentId])
REFERENCES [dbo].[Assessment] ([Id])
GO
ALTER TABLE [dbo].[AssessmentQuestion] CHECK CONSTRAINT [FK_AssessmentQuestion_AssessmentId]
GO
ALTER TABLE [dbo].[AssessmentQuestion]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentQuestion_TextId] FOREIGN KEY([TextId])
REFERENCES [dbo].[Resource] ([Id])
GO
ALTER TABLE [dbo].[AssessmentQuestion] CHECK CONSTRAINT [FK_AssessmentQuestion_TextId]
GO
ALTER TABLE [dbo].[AssessmentQuestion]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentQuestion_ValidationTextId] FOREIGN KEY([ValidationTextId])
REFERENCES [dbo].[Resource] ([Id])
GO
ALTER TABLE [dbo].[AssessmentQuestion] CHECK CONSTRAINT [FK_AssessmentQuestion_ValidationTextId]
GO
ALTER TABLE [dbo].[AssessmentQuestion]  WITH CHECK ADD  CONSTRAINT [FK_SurveyQuestion_SectionId] FOREIGN KEY([SectionId])
REFERENCES [dbo].[Section] ([Id])
GO
ALTER TABLE [dbo].[AssessmentQuestion] CHECK CONSTRAINT [FK_SurveyQuestion_SectionId]
GO
ALTER TABLE [dbo].[Partner]  WITH CHECK ADD  CONSTRAINT [FK_Partner_PartnerDetails] FOREIGN KEY([PartnerDetailsId])
REFERENCES [dbo].[PartnerDetails] ([Id])
GO
ALTER TABLE [dbo].[Partner] CHECK CONSTRAINT [FK_Partner_PartnerDetails]
GO
ALTER TABLE [dbo].[Question]  WITH CHECK ADD  CONSTRAINT [FK_Question_AnswerType] FOREIGN KEY([AnswerTypeId])
REFERENCES [dbo].[AnswerType] ([Id])
GO
ALTER TABLE [dbo].[Question] CHECK CONSTRAINT [FK_Question_AnswerType]
GO
ALTER TABLE [dbo].[Question]  WITH CHECK ADD  CONSTRAINT [FK_Question_ResourceId] FOREIGN KEY([ResourceId])
REFERENCES [dbo].[Resource] ([Id])
GO
ALTER TABLE [dbo].[Question] CHECK CONSTRAINT [FK_Question_ResourceId]
GO
ALTER TABLE [dbo].[QuestionItem]  WITH CHECK ADD  CONSTRAINT [FK_QuestionItem_AnswerType] FOREIGN KEY([AnswerTypeId])
REFERENCES [dbo].[AnswerType] ([Id])
GO
ALTER TABLE [dbo].[QuestionItem] CHECK CONSTRAINT [FK_QuestionItem_AnswerType]
GO
ALTER TABLE [dbo].[QuestionItem]  WITH CHECK ADD  CONSTRAINT [FK_QuestionItem_QuestionId] FOREIGN KEY([QuestionId])
REFERENCES [dbo].[Question] ([Id])
GO
ALTER TABLE [dbo].[QuestionItem] CHECK CONSTRAINT [FK_QuestionItem_QuestionId]
GO
ALTER TABLE [dbo].[QuestionItem]  WITH CHECK ADD  CONSTRAINT [FK_QuestionItem_ResourceId] FOREIGN KEY([ResourceId])
REFERENCES [dbo].[Resource] ([Id])
GO
ALTER TABLE [dbo].[QuestionItem] CHECK CONSTRAINT [FK_QuestionItem_ResourceId]
GO
ALTER TABLE [dbo].[Resource]  WITH CHECK ADD  CONSTRAINT [FK_Resource_ResourceTypeId] FOREIGN KEY([ResourceTypeId])
REFERENCES [dbo].[ResourceType] ([Id])
GO
ALTER TABLE [dbo].[Resource] CHECK CONSTRAINT [FK_Resource_ResourceTypeId]
GO
ALTER TABLE [dbo].[ResourceValue]  WITH CHECK ADD  CONSTRAINT [FK_ResourceValue_ResourceId] FOREIGN KEY([ResourceId])
REFERENCES [dbo].[Resource] ([Id])
GO
ALTER TABLE [dbo].[ResourceValue] CHECK CONSTRAINT [FK_ResourceValue_ResourceId]
GO
ALTER TABLE [dbo].[Section]  WITH CHECK ADD  CONSTRAINT [FK_Section_ResourceId] FOREIGN KEY([ResourceId])
REFERENCES [dbo].[Resource] ([Id])
GO
ALTER TABLE [dbo].[Section] CHECK CONSTRAINT [FK_Section_ResourceId]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Partner] FOREIGN KEY([PartnerId])
REFERENCES [dbo].[Partner] ([Id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Partner]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Status] FOREIGN KEY([StatusId])
REFERENCES [dbo].[UserStatus] ([Id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Status]
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_Role]
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_User]
GO
/****** Object:  StoredProcedure [dbo].[usp_AddQuestion]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_AddQuestion]
	@aName nvarchar(50),				-- name of the main assessment
	@taName nvarchar(50) = NULL,		-- name of the translated assessment (optional)
	@answerTypeName nvarchar(50),

	@qNo int,
	@qText nvarchar(4000),
	@qAltText nvarchar(4000) = NULL,	-- placeholder text in DDL such as 'Please select' (optional)
	@qValText nvarchar(4000) = NULL,	-- validation text (optional)

	@sectionNo int,						-- section number
	@sectionText nvarchar(4000) = NULL,	-- text to appear as section name or description (optional)

	@acNo int,							-- answer choice order
	@aacNo int,							-- can be used to change order
	@acName nvarchar(200) = NULL,		-- Address, Email, Country, Industry, Other, None of the above etc. (optional)
	@aacText nvarchar(4000) = NULL,		-- dropdown: Ar=gentina, Austria, etc., all other types: answer choice text, Other, None of the above etc.  (optional)
	@link nvarchar(40) = NULL,			-- indicates a link to an alternative answer choice in DDL such as Other  (optional)
	@isAltChoice nvarchar(40) = NULL,	-- indicates that this is an alternative answer choice such as Other, None of the above ect. (optional)
	@score decimal,

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
	DECLARE @questionTypeId int

	DECLARE @sectionName nvarchar(50)
	DECLARE @sectionId bigint
	DECLARE @srId bigint

	DECLARE @qiId bigint
	DECLARE @qiName nvarchar(50)
	DECLARE @qiDescription nvarchar(200)

	DECLARE @acId bigint

	DECLARE @aacname nvarchar(50)
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
	-- Answer Type
	select @answerTypeId = Id from AnswerType where Name = @answerTypeName
	IF @answerTypeId IS NULL return

	--*****************************************************************************
	-- Question (q)
	SET @qName =  'A' + CONVERT(nvarchar, @aId) + '-' + 'Q' + CONVERT(nvarchar, @qNo)

	select @qId = Id, @qrId = ResourceId, @questionTypeId = AnswerTypeId from Question where Name = @qName

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

		SET @questionTypeId = @answerTypeId
		insert into Question (Name, ResourceId, AnswerTypeId, [Description]) values (@qName, @qrId, @questionTypeId, @qText)
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
		where AssessmentQuestionId = @aqId and AltQuestionItemId = 1
	END

	if @@error <> 0 return

	--*****************************************************************************
	-- AnswerChoice (ac) & AssesmenAnswerChoice (aac)
	SET @aacName = @qiName + '-' + CONVERT(nvarchar, @acNo)
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
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetHash]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_GetHash]
	@Cleartext	NVARCHAR(4000)
AS
BEGIN	
	SELECT CONVERT(NVARCHAR(32), HASHBYTES('SHA2_256', @Cleartext), 2)
END



GO
/****** Object:  StoredProcedure [dbo].[usp_SetResourceValue]    Script Date: 11/5/2015 4:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_SetResourceValue]
	@key nvarchar(200),
	@value nvarchar(4000),
	@culture nvarchar(50) = 'en-US',
	@type int = 1,
	@tag nvarchar(100) = null
AS
BEGIN

	SET NOCOUNT ON

	DECLARE @exists int = 0

	IF NOT EXISTS (select 1 from Resource where Name = @key)
	BEGIN		
		INSERT INTO Resource (Name, DefaultValue, ResourceTypeId, Tag) VALUES (@key, @value, @type, @tag)
	END
	ELSE
	BEGIN
		IF @culture = 'en-US'
		BEGIN
			update Resource set DefaultValue = @value, ResourceTypeId = @type, Tag = @tag, DateUpdated = getdate() where Name = @key
		END

		update rv set rv.Value = @value, rv.DateUpdated = getdate()
		from ResourceValue rv join Resource r on rv.ResourceId = r.Id
		where Name = @key and CultureName = @culture

		SET @exists = @@ROWCOUNT
	END

	IF @exists <= 0
	BEGIN
		INSERT INTO ResourceValue (Value, CultureName, ResourceId)
		select @value as Value, @culture as CultureName, Id as ResourceId from Resource where Name = @key
	END

END
GO
USE [master]
GO
ALTER DATABASE [943078_db_ricohioi_a] SET  READ_WRITE 
GO
