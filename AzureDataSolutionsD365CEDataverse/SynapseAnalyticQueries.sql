--Create unique schema

CREATE SCHEMA AZDataTalk;

--Create table for data load

CREATE TABLE AZDataTalk.stg_CompaniesHouse
(
	accountid UNIQUEIDENTIFIER NULL,
	CompanyName NVARCHAR(MAX) NOT NULL,
	CompanyNumber NVARCHAR(MAX) NOT NULL,
	RegAddress_PostCode NVARCHAR(MAX) NULL,
	CompanyCategory NVARCHAR(MAX) NULL,
	IncorporationDate NVARCHAR(MAX) NULL,
	Accounts_AccountRefDay NVARCHAR(MAX) NULL,
	Accounts_AccountRefMonth NVARCHAR(MAX) NULL,
	Accounts_NextDueDate NVARCHAR(MAX) NULL,
	Accounts_LastMadeUpDate NVARCHAR(MAX) NULL,
	Accounts_AccountCategory NVARCHAR(MAX) NULL,
	Returns_NextDueDate NVARCHAR(MAX) NULL,
	Returns_LastMadeUpDate NVARCHAR(MAX) NULL,
	Mortgages_NumMortCharges NVARCHAR(MAX) NULL,
	Mortgages_NumMortOutstanding NVARCHAR(MAX) NULL,
	Mortgages_NumMortPartSatisfied NVARCHAR(MAX) NULL,
	Mortgages_NumMortSatisfied NVARCHAR(MAX) NULL,
	SICCode_SicText_1 NVARCHAR(MAX) NULL,
	SICCode_SicText_2 NVARCHAR(MAX) NULL,
	SICCode_SicText_3 NVARCHAR(MAX) NULL,
	SICCode_SicText_4 NVARCHAR(MAX) NULL,
	LimitedPartnerships_NumGenPartners NVARCHAR(MAX) NULL,
	LimitedPartnerships_NumLimPartners NVARCHAR(MAX) NULL,
	URI NVARCHAR(MAX) NULL,
	ConfStmtNextDueDate NVARCHAR(MAX) NULL,
	ConfStmtLastMadeUpDate NVARCHAR(MAX) NULL
)
WITH
(
	DISTRIBUTION = ROUND_ROBIN,
	HEAP
)
GO

CREATE TABLE AZDataTalk.CompaniesHouse
(
	accountid UNIQUEIDENTIFIER NULL,
	CompanyName NVARCHAR(160) NOT NULL,
	CompanyNumber NVARCHAR(68) NOT NULL,
	RegAddress_PostCode NVARCHAR(10) NULL,
	CompanyCategory NVARCHAR(89) NULL,
	IncorporationDate DATE NULL,
	Accounts_AccountRefDay INT NULL,
	Accounts_AccountRefMonth INT NULL,
	Accounts_NextDueDate DATE NULL,
	Accounts_LastMadeUpDate DATE NULL,
	Accounts_AccountCategory NVARCHAR(27) NULL,
	Returns_NextDueDate DATE NULL,
	Returns_LastMadeUpDate DATE NULL,
	Mortgages_NumMortCharges NVARCHAR(10) NULL,
	Mortgages_NumMortOutstanding NVARCHAR(10) NULL,
	Mortgages_NumMortPartSatisfied INT NULL,
	Mortgages_NumMortSatisfied NVARCHAR(40) NULL,
	SICCode_SicText_1 NVARCHAR(166) NULL,
	SICCode_SicText_2 NVARCHAR(166) NULL,
	SICCode_SicText_3 NVARCHAR(166) NULL,
	SICCode_SicText_4 NVARCHAR(166) NULL,
	LimitedPartnerships_NumGenPartners NVARCHAR(64) NULL,
	LimitedPartnerships_NumLimPartners NVARCHAR(47) NULL,
	URI NVARCHAR(47) NULL,
	ConfStmtNextDueDate DATE NULL,
	ConfStmtLastMadeUpDate DATE NULL
)
WITH
(
	DISTRIBUTION = HASH(CompanyNumber),
	CLUSTERED COLUMNSTORE INDEX
)
GO

CREATE TABLE AZDataTalk.stg_CompaniesHouseAggregates
(
	SICCode_SicText_1 NVARCHAR(MAX) NOT NULL,
	revenue MONEY NULL,
	[count] INT NULL
)
WITH
(
	DISTRIBUTION = ROUND_ROBIN,
	HEAP
)
GO

CREATE TABLE AZDataTalk.CompaniesHouseAggregates
(
	SICCode_SicText_1 NVARCHAR(250) NOT NULL,
	revenue MONEY NULL,
	[count] INT NULL
)
WITH
(
	DISTRIBUTION = HASH(SICCode_SicText_1),
	CLUSTERED COLUMNSTORE INDEX
);
GO

CREATE PROCEDURE AZDataTalk.usp_MergeCompaniesHouseAggregates
AS
MERGE AZDataTalk.CompaniesHouseAggregates AS target
USING AZDataTalk.stg_CompaniesHouseAggregates AS source
ON target.SICCode_SicText_1 = source.SICCode_SicText_1
WHEN MATCHED THEN
	UPDATE
	SET target.revenue = source.revenue,
		target.[count] = source.[count]
WHEN NOT MATCHED THEN
	INSERT (SICCode_SicText_1, revenue, [count])
	VALUES (source.SICCode_SicText_1, source.revenue, source.[count]);
GO

CREATE PROCEDURE AZDataTalk.usp_MergeCompaniesHouse
AS
--Manually handle any problematic records by deleting them first
DELETE FROM [AZDataTalk].[stg_CompaniesHouse]
WHERE LEN(CompanyNumber) >= 9;
DELETE FROM [AZDataTalk].[stg_CompaniesHouse]
WHERE LEN([IncorporationDate]) >= 11;
DELETE FROM [AZDataTalk].[stg_CompaniesHouse]
WHERE LEN([Accounts_LastMadeUpDate]) >= 11;
DELETE FROM [AZDataTalk].[stg_CompaniesHouse]
WHERE LEN([Returns_NextDueDate]) >= 11;

DELETE FROM [AZDataTalk].[stg_CompaniesHouse]
WHERE LEN([IncorporationDate]) <= 9;
DELETE FROM [AZDataTalk].[stg_CompaniesHouse]
WHERE LEN([Accounts_LastMadeUpDate]) <= 9;
DELETE FROM [AZDataTalk].[stg_CompaniesHouse]
WHERE LEN([Returns_NextDueDate]) <= 9;

DELETE FROM [AZDataTalk].[stg_CompaniesHouse]
WHERE LEN([Accounts_AccountRefDay]) = 10;

DELETE FROM [AZDataTalk].[stg_CompaniesHouse]
WHERE LEN([RegAddress_PostCode]) >= 10;

MERGE AZDataTalk.CompaniesHouse AS target
USING AZDataTalk.stg_CompaniesHouse AS source
ON target.CompanyNumber = source.CompanyNumber
WHEN MATCHED THEN
	UPDATE
	SET target.accountid = source.accountid,
		target.CompanyName = source.CompanyName,
		target.RegAddress_PostCode = source.RegAddress_PostCode,
		target.CompanyCategory = source.CompanyCategory,
		target.IncorporationDate = TRY_PARSE(source.IncorporationDate AS DATE USING 'en-gb'),
		target.Accounts_AccountRefDay = source.Accounts_AccountRefDay,
		target.Accounts_AccountRefMonth = source.Accounts_AccountRefMonth,
		target.Accounts_NextDueDate = TRY_PARSE(source.Accounts_NextDueDate AS DATE USING 'en-gb'),
		target.Accounts_LastMadeUpDate = TRY_PARSE(source.Accounts_LastMadeUpDate AS DATE USING 'en-gb'),
		target.Accounts_AccountCategory = source.Accounts_AccountCategory,
		target.Returns_NextDueDate = TRY_PARSE(source.Returns_NextDueDate AS DATE USING 'en-gb'),
		target.Returns_LastMadeUpDate = TRY_PARSE(source.Returns_LastMadeUpDate AS DATE USING 'en-gb'),
		target.Mortgages_NumMortCharges = source.Mortgages_NumMortCharges,
		target.Mortgages_NumMortOutstanding = source.Mortgages_NumMortOutstanding,
		target.Mortgages_NumMortPartSatisfied = source.Mortgages_NumMortPartSatisfied,
		target.Mortgages_NumMortSatisfied = source.Mortgages_NumMortSatisfied ,
		target.SICCode_SicText_1 = source.SICCode_SicText_1,
		target.SICCode_SicText_2 = source.SICCode_SicText_2,
		target.SICCode_SicText_3 = source.SICCode_SicText_3,
		target.SICCode_SicText_4 = source.SICCode_SicText_4,
		target.LimitedPartnerships_NumGenPartners = source.LimitedPartnerships_NumGenPartners,
		target.LimitedPartnerships_NumLimPartners = source.LimitedPartnerships_NumLimPartners,
		target.URI = source.URI,
		target.ConfStmtNextDueDate = TRY_PARSE(source.ConfStmtNextDueDate AS DATE USING 'en-gb'),
		target.ConfStmtLastMadeUpDate = TRY_PARSE(source.ConfStmtLastMadeUpDate AS DATE USING 'en-gb')
WHEN NOT MATCHED THEN
	INSERT (accountid, CompanyName, RegAddress_PostCode, CompanyNumber, 
			CompanyCategory, IncorporationDate, Accounts_AccountRefDay, Accounts_AccountRefMonth, 
			Accounts_NextDueDate, Accounts_LastMadeUpDate, Accounts_AccountCategory, Returns_NextDueDate, 
			Returns_LastMadeUpDate, Mortgages_NumMortCharges, Mortgages_NumMortOutstanding, Mortgages_NumMortPartSatisfied, 
			Mortgages_NumMortSatisfied, SICCode_SicText_1, SICCode_SicText_2, SICCode_SicText_3, 
			SICCode_SicText_4, LimitedPartnerships_NumGenPartners, LimitedPartnerships_NumLimPartners, URI, 
			ConfStmtNextDueDate, ConfStmtLastMadeUpDate)
	VALUES (source.accountid, source.CompanyName, source.RegAddress_PostCode, source.CompanyNumber, 
			source.CompanyCategory, TRY_PARSE(source.IncorporationDate AS DATE USING 'en-gb'), source.Accounts_AccountRefDay, source.Accounts_AccountRefMonth, 
			TRY_PARSE(source.Accounts_NextDueDate AS DATE USING 'en-gb'), TRY_PARSE(source.Accounts_LastMadeUpDate AS DATE USING 'en-gb'), source.Accounts_AccountCategory, TRY_PARSE(source.Returns_NextDueDate AS DATE USING 'en-gb'), 
			TRY_PARSE(source.Returns_LastMadeUpDate AS DATE USING 'en-gb'), source.Mortgages_NumMortCharges, source.Mortgages_NumMortOutstanding, source.Mortgages_NumMortPartSatisfied, 
			source.Mortgages_NumMortSatisfied, source.SICCode_SicText_1, source.SICCode_SicText_2, source.SICCode_SicText_3, 
			source.SICCode_SicText_4, source.LimitedPartnerships_NumGenPartners, source.LimitedPartnerships_NumLimPartners, source.URI, 
			TRY_PARSE(source.ConfStmtNextDueDate AS DATE USING 'en-gb'), TRY_PARSE(source.ConfStmtLastMadeUpDate AS DATE USING 'en-gb'));
GO