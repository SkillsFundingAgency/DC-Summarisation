﻿CREATE TABLE [dbo].[SummarisedActuals](
    [ID]						INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CollectionReturnId]		INT NOT NULL,
    [OrganisationId]            VARCHAR(10) NOT NULL,
    [UoPCode]                   VARCHAR(20) NULL,
    [FundingStreamPeriodCode]   VARCHAR(20) NOT NULL,
    [Period]                    INT NOT NULL,
    [DeliverableCode]           INT NOT NULL,
    [ActualVolume]              INT NOT NULL,
    [ActualValue]               DECIMAL(13,2) NOT NULL,
    [PeriodTypeCode]            VARCHAR(10) NOT NULL,
    [ContractAllocationNumber]  VARCHAR(20) NULL, 
    CONSTRAINT [FK_SummarisedActuals_CollectionReturn] FOREIGN KEY ([CollectionReturnId]) REFERENCES CollectionReturn([Id]),
)
GO
CREATE NONCLUSTERED INDEX [IX_SA_CollectionReturnId_OrgId] ON [dbo].[SummarisedActuals]
(
	[CollectionReturnId] ASC,
	[OrganisationId] ASC
)
GO
CREATE NONCLUSTERED INDEX [IDX_SummarisedActuals_API]
ON [dbo].[SummarisedActuals] ([CollectionReturnId])
INCLUDE ([OrganisationId],[UoPCode],[FundingStreamPeriodCode],[Period],[DeliverableCode],[ActualVolume],[ActualValue],[PeriodTypeCode],[ContractAllocationNumber])
 WITH (DATA_COMPRESSION = PAGE, SORT_IN_TEMPDB = ON, STATISTICS_NORECOMPUTE = OFF, ONLINE = ON)
GO
