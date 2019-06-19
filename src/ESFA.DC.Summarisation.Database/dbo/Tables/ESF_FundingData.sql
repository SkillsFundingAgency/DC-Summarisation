CREATE TABLE [dbo].[ESF_FundingData]
(
	[UKPRN] [int] NOT NULL,
	[ConRefNumber] [varchar](20) NOT NULL,
	[DeliverableCode] [varchar](5) NOT NULL,
	[AttributeName] [varchar](100) NOT NULL,
	[CollectionYear] [int] NOT NULL,
	[CollectionPeriod] [int] NOT NULL,
	[Period_1] [decimal](15, 5) NULL,
	[Period_2] [decimal](15, 5) NULL,
	[Period_3] [decimal](15, 5) NULL,
	[Period_4] [decimal](15, 5) NULL,
	[Period_5] [decimal](15, 5) NULL,
	[Period_6] [decimal](15, 5) NULL,
	[Period_7] [decimal](15, 5) NULL,
	[Period_8] [decimal](15, 5) NULL,
	[Period_9] [decimal](15, 5) NULL,
	[Period_10] [decimal](15, 5) NULL,
	[Period_11] [decimal](15, 5) NULL,
	[Period_12] [decimal](15, 5) NULL,
)
GO
ALTER TABLE [ESF_FundingData] ADD PRIMARY KEY CLUSTERED 
(
	[UKPRN] ASC,
	[ConRefNumber] ASC,
	[DeliverableCode] ASC,
	[AttributeName] ASC,
	[CollectionYear] ASC,
	[CollectionPeriod] ASC
)
GO