USE [master]
GO
/****** Object:  Database [MoreRulesEngine]    Script Date: 12/13/2012 15:37:29 ******/
CREATE DATABASE [MoreRulesEngine] 
GO
ALTER DATABASE [MoreRulesEngine] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [MoreRulesEngine].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [MoreRulesEngine] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [MoreRulesEngine] SET ANSI_NULLS OFF
GO
ALTER DATABASE [MoreRulesEngine] SET ANSI_PADDING OFF
GO
ALTER DATABASE [MoreRulesEngine] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [MoreRulesEngine] SET ARITHABORT OFF
GO
ALTER DATABASE [MoreRulesEngine] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [MoreRulesEngine] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [MoreRulesEngine] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [MoreRulesEngine] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [MoreRulesEngine] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [MoreRulesEngine] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [MoreRulesEngine] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [MoreRulesEngine] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [MoreRulesEngine] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [MoreRulesEngine] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [MoreRulesEngine] SET  DISABLE_BROKER
GO
ALTER DATABASE [MoreRulesEngine] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [MoreRulesEngine] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [MoreRulesEngine] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [MoreRulesEngine] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [MoreRulesEngine] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [MoreRulesEngine] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [MoreRulesEngine] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [MoreRulesEngine] SET  READ_WRITE
GO
ALTER DATABASE [MoreRulesEngine] SET RECOVERY FULL
GO
ALTER DATABASE [MoreRulesEngine] SET  MULTI_USER
GO
ALTER DATABASE [MoreRulesEngine] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [MoreRulesEngine] SET DB_CHAINING OFF
GO
EXEC sys.sp_db_vardecimal_storage_format N'MoreRulesEngine', N'ON'
GO
USE [MoreRulesEngine]
GO
/****** Object:  Table [dbo].[RuleAssemblies]    Script Date: 12/13/2012 15:37:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RuleAssemblies](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AssemblyName] [nvarchar](255) NOT NULL,
	[EffectiveDate] [datetime] NOT NULL,
	[Published] [bit] NOT NULL,
	[LastCompileDate] [datetime] NULL,
 CONSTRAINT [PK_RatingAssemblies] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RuleBooks]    Script Date: 12/13/2012 15:37:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RuleBooks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[ChangeId] [uniqueidentifier] NOT NULL,
	[EffectiveDate] [datetime] NOT NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_RatingExceptions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RuleBookRules]    Script Date: 12/13/2012 15:37:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RuleBookRules](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ChangeId] [uniqueidentifier] NULL,
	[EffectiveDate] [datetime] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[RuleExpression] [nvarchar](max) NULL,
	[ParentChangeId] [uniqueidentifier] NULL,
	[Tag] [nvarchar](10) NULL,
	[Context] [nvarchar](25) NULL,
	[Active] [bit] NOT NULL,
	[RuleBookId] [int] NOT NULL,
 CONSTRAINT [PK_RuleBookRules] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[fnRuleBookRules]    Script Date: 12/13/2012 15:37:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fnRuleBookRules] (	
  @EffectiveDate [datetime]
)
RETURNS TABLE
AS
RETURN (
    --DECLARE @EffectiveDate [datetime]
    --SET @EffectiveDate = '2012-05-01'

    SELECT
        *
    FROM [dbo].[RuleBookRules] ft
    WHERE ft.[Id] IN (
            SELECT
                MAX(ft1.[Id]) [Id]
            FROM [dbo].[RuleBookRules] ft1
            WHERE ft1.[Id] IN (
                    SELECT
                        ft2.[Id] [Id]
                    FROM [dbo].[RuleBookRules] ft2 
                    WHERE ft2.[EffectiveDate] <= @EffectiveDate
              
                )
             
            GROUP BY ft1.[ChangeId]
        )
     
    -- ORDER BY ft.[Id] ASC -- (Not valid in views, inline functions,...)
)
GO
/****** Object:  StoredProcedure [dbo].[GetRuleBookRules]    Script Date: 12/13/2012 15:37:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetRuleBookRules]
	-- Add the parameters for the stored procedure here
	@EffectiveDate datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM fnRuleBookRules(@EffectiveDate) --WHERE BaseKeyId = @BaseKeyId
	
END
GO
/****** Object:  Default [DF_RatingAssemblies_Published]    Script Date: 12/13/2012 15:37:31 ******/
ALTER TABLE [dbo].[RuleAssemblies] ADD  CONSTRAINT [DF_RatingAssemblies_Published]  DEFAULT ((0)) FOR [Published]
GO
/****** Object:  Default [DF_RuleBooks_ChangeId]    Script Date: 12/13/2012 15:37:31 ******/
ALTER TABLE [dbo].[RuleBooks] ADD  CONSTRAINT [DF_RuleBooks_ChangeId]  DEFAULT (newid()) FOR [ChangeId]
GO
/****** Object:  Default [DF_RatingExceptions_Active]    Script Date: 12/13/2012 15:37:31 ******/
ALTER TABLE [dbo].[RuleBooks] ADD  CONSTRAINT [DF_RatingExceptions_Active]  DEFAULT ((1)) FOR [Active]
GO
/****** Object:  Default [DF_RatingRules_Active]    Script Date: 12/13/2012 15:37:31 ******/
ALTER TABLE [dbo].[RuleBookRules] ADD  CONSTRAINT [DF_RatingRules_Active]  DEFAULT ((1)) FOR [Active]
GO
/****** Object:  Default [DF_RuleBookRules_RuleBookId]    Script Date: 12/13/2012 15:37:31 ******/
ALTER TABLE [dbo].[RuleBookRules] ADD  CONSTRAINT [DF_RuleBookRules_RuleBookId]  DEFAULT ((0)) FOR [RuleBookId]
GO
/****** Object:  ForeignKey [FK_RuleBookRules_RuleBooks]    Script Date: 12/13/2012 15:37:31 ******/
ALTER TABLE [dbo].[RuleBookRules]  WITH CHECK ADD  CONSTRAINT [FK_RuleBookRules_RuleBooks] FOREIGN KEY([RuleBookId])
REFERENCES [dbo].[RuleBooks] ([Id])
GO
ALTER TABLE [dbo].[RuleBookRules] CHECK CONSTRAINT [FK_RuleBookRules_RuleBooks]
GO
