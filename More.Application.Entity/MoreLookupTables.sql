USE [master]
GO
/****** Object:  Database [MoreLookupTables]    Script Date: 12/13/2012 15:40:36 ******/
CREATE DATABASE [MoreLookupTables] 
GO
ALTER DATABASE [MoreLookupTables] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [MoreLookupTables].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [MoreLookupTables] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [MoreLookupTables] SET ANSI_NULLS OFF
GO
ALTER DATABASE [MoreLookupTables] SET ANSI_PADDING OFF
GO
ALTER DATABASE [MoreLookupTables] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [MoreLookupTables] SET ARITHABORT OFF
GO
ALTER DATABASE [MoreLookupTables] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [MoreLookupTables] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [MoreLookupTables] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [MoreLookupTables] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [MoreLookupTables] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [MoreLookupTables] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [MoreLookupTables] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [MoreLookupTables] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [MoreLookupTables] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [MoreLookupTables] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [MoreLookupTables] SET  DISABLE_BROKER
GO
ALTER DATABASE [MoreLookupTables] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [MoreLookupTables] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [MoreLookupTables] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [MoreLookupTables] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [MoreLookupTables] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [MoreLookupTables] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [MoreLookupTables] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [MoreLookupTables] SET  READ_WRITE
GO
ALTER DATABASE [MoreLookupTables] SET RECOVERY FULL
GO
ALTER DATABASE [MoreLookupTables] SET  MULTI_USER
GO
ALTER DATABASE [MoreLookupTables] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [MoreLookupTables] SET DB_CHAINING OFF
GO
EXEC sys.sp_db_vardecimal_storage_format N'MoreLookupTables', N'ON'
GO
USE [MoreLookupTables]
/****** Object:  UserDefinedFunction [dbo].[fnGetFactorTableColumns]    Script Date: 12/13/2012 15:40:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[fnGetFactorTableColumns]
()
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	select 
	TABLE_NAME,COLUMN_NAME,
	(select value from fn_listextendedproperty (NULL, 'schema', c.TABLE_SCHEMA, 'table', c.TABLE_NAME, 'column', c.COLUMN_NAME) where name='IsKey') as IsKey,
	(select value from fn_listextendedproperty (NULL, 'schema', c.TABLE_SCHEMA, 'table', c.TABLE_NAME, 'column', c.COLUMN_NAME) where name='LookupType' ) as LookupType
	
	from INFORMATION_SCHEMA.COLUMNS c
)
GO
/****** Object:  UserDefinedFunction [dbo].[fnGetAllFactorTables]    Script Date: 12/13/2012 15:40:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[fnGetAllFactorTables]
(	
)
RETURNS TABLE 
AS
RETURN 
(

	-- Add the SELECT statement with parameter references here
	select 
	c.TABLE_NAME,
	
	CAST((select value from fn_listextendedproperty (NULL, 'schema', 'dbo', 'table', c.TABLE_NAME,null,null) where name = 'Id') as int ) as Id,
	CAST((select value from fn_listextendedproperty (NULL, 'schema', 'dbo', 'table', c.TABLE_NAME,null,null) where name = 'Active') as bit ) as Active,
	CAST((select value from fn_listextendedproperty (NULL, 'schema', 'dbo', 'table', c.TABLE_NAME,null,null) where name = 'CreateDate') as datetime ) as CreateDate,
	CAST((select value from fn_listextendedproperty (NULL, 'schema', 'dbo', 'table', c.TABLE_NAME,null,null) where name = 'ChangeId') as uniqueidentifier ) as ChangeId,
	CAST((select value from fn_listextendedproperty (NULL, 'schema', 'dbo', 'table', c.TABLE_NAME,null,null) where name = 'EffectiveDate') as datetime ) as EffectiveDate
	from INFORMATION_SCHEMA.TABLES c
	
)
GO
/****** Object:  StoredProcedure [dbo].[GetAllFactorTables]    Script Date: 12/13/2012 15:40:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetAllFactorTables]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * from fnGetAllFactorTables() order by Id
END
GO
/****** Object:  UserDefinedFunction [dbo].[fnGetFactorTables]    Script Date: 12/13/2012 15:40:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[fnGetFactorTables]
(	
	@EffectiveDate [datetime]
)
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
 SELECT
		Id,
        TABLE_NAME as TableName,
        CAST(EffectiveDate as datetime)as EffectiveDate,
        CreateDate,
        ChangeId,
        Active
    FROM fnGetAllFactorTables() ft
    WHERE ft.[Id] IN (
            SELECT
                MAX(ft1.[Id]) [Id]
            FROM fnGetAllFactorTables() ft1
            WHERE ft1.[Id] IN (
                    SELECT
                        ft2.[Id] [Id]
                    FROM fnGetAllFactorTables() ft2 
                    WHERE ft2.[EffectiveDate] <= @EffectiveDate
					
                )
             
            GROUP BY ft1.[ChangeId]
        )
)
GO
/****** Object:  StoredProcedure [dbo].[GetTableColumns]    Script Date: 12/13/2012 15:40:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetTableColumns]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    select TABLE_NAME as TableName, COLUMN_NAME as ColumnName, CAST(IsKey as bit) as IsKey, CAST(LookupType as nvarchar(128)) as LookupType from fnGetFactorTableColumns() where IsKey IS NOT NULL
END
GO
/****** Object:  StoredProcedure [dbo].[GetNextFactorTableId]    Script Date: 12/13/2012 15:40:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetNextFactorTableId]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
		SELECT MAX(Id) + 1 as Column1 from fnGetAllFactorTables()
END
GO
/****** Object:  StoredProcedure [dbo].[GetFactorTables]    Script Date: 12/13/2012 15:40:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetFactorTables]
	-- Add the parameters for the stored procedure here
	@EffectiveDate datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * from fnGetFactorTables(@EffectiveDate) ft
END
GO
