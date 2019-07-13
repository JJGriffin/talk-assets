SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Joe Griffin
-- Create date: 19th April 2017
-- Description: User defined function to return an Option Set field label value based on input parameters
-- =============================================
CREATE FUNCTION [dbo].[fnGetGlobalOptionSetLabel]
(
	@GlobalOptionSetName NVARCHAR(64),
	@Option INT,
	@LanguageCode INT
)
RETURNS NVARCHAR(256)
AS
BEGIN

	DECLARE @Label NVARCHAR(256);
	DECLARE @RecordCount INT = (SELECT COUNT(*) FROM dbo.GlobalOptionSetMetadata WHERE OptionSetName = @GlobalOptionSetName AND [Option] = @Option AND LocalizedLabelLanguageCode = @LanguageCode);
	IF @RecordCount = 1
		SET @Label = (SELECT TOP 1 LocalizedLabel FROM dbo.GlobalOptionSetMetadata WHERE OptionSetName = @GlobalOptionSetName AND [Option] = @Option AND LocalizedLabelLanguageCode = @LanguageCode);
	ELSE
		SET @Label = CAST('An error has occurred. Could not obtain label for Global Option Set field ' + @GlobalOptionSetName AS INT);
	RETURN @Label;

END

GO

-- =============================================
-- Author:		Joe Griffin
-- Create date: 19th April 2017
-- Description: User defined function to return an Option Set field label value based on input parameters
-- =============================================
CREATE FUNCTION [dbo].[fnGetOptionSetLabel]
(
	@EntityName NVARCHAR(64), 
	@OptionSetName NVARCHAR(64),
	@Option INT,
	@LanguageCode INT
)
RETURNS NVARCHAR(256)
AS
BEGIN

	DECLARE @Label NVARCHAR(256);
	DECLARE @RecordCount INT = (SELECT COUNT(*) FROM dbo.OptionSetMetadata WHERE EntityName = @EntityName AND OptionSetName = @OptionSetName AND [Option] = @Option AND LocalizedLabelLanguageCode = @LanguageCode);
	IF @RecordCount = 1
		SET @Label = (SELECT TOP 1 LocalizedLabel FROM dbo.OptionSetMetadata WHERE EntityName = @EntityName AND OptionSetName = @OptionSetName AND [Option] = @Option AND LocalizedLabelLanguageCode = @LanguageCode);
	ELSE
		SET @Label = CAST('An error has occurred. Could not obtain label for Option Set field ' + @OptionSetName AS INT);
	RETURN @Label;

END

GO

-- =============================================
-- Author:		Joe Griffin
-- Create date: 19th April 2017
-- Description: User defined function to return an State field label value based on input parameters
-- =============================================
CREATE FUNCTION [dbo].[fnGetStateLabel]
(
	@EntityName NVARCHAR(64),
	@State INT,
	@LanguageCode INT
)
RETURNS NVARCHAR(256)
AS
BEGIN

	DECLARE @Label NVARCHAR(256);
	DECLARE @RecordCount INT = (SELECT COUNT(*) FROM dbo.StateMetadata WHERE EntityName = @EntityName AND [State] = @State AND LocalizedLabelLanguageCode = @LanguageCode);
	IF @RecordCount = 1
		SET @Label = (SELECT TOP 1 LocalizedLabel FROM dbo.StateMetadata WHERE EntityName = @EntityName AND [State] = @State AND LocalizedLabelLanguageCode = @LanguageCode);
	ELSE
		SET @Label = CAST('An error has occurred. Could not obtain State label for entity ' + @EntityName AS INT);
	RETURN @Label;

END

GO

-- =============================================
-- Author:		Joe Griffin
-- Create date: 19th April 2017
-- Description: User defined function to return a Status field label value based on input parameters
-- =============================================
CREATE FUNCTION [dbo].[fnGetStatusLabel]
(
	@EntityName NVARCHAR(64),
	@Status INT,
	@LanguageCode INT
)
RETURNS NVARCHAR(256)
AS
BEGIN

	DECLARE @Label NVARCHAR(256);
	DECLARE @RecordCount INT = (SELECT COUNT(*) FROM dbo.StatusMetadata WHERE EntityName = @EntityName AND [Status] = @Status AND LocalizedLabelLanguageCode = @LanguageCode);
	IF @RecordCount = 1
		SET @Label = (SELECT TOP 1 LocalizedLabel FROM dbo.StatusMetadata WHERE EntityName = @EntityName AND [Status] = @Status AND LocalizedLabelLanguageCode = @LanguageCode);
	ELSE
		SET @Label = CAST('An error has occurred. Could not obtain Status label for entity ' + @EntityName AS INT);
	RETURN @Label;

END
GO
