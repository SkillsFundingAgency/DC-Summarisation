CREATE TABLE [dbo].[SummarisedActuals](
    [ID]						INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    [CollectionType]            VARCHAR(20) NOT NULL,
    [CollectionReturnCode]      VARCHAR(10) NOT NULL,
    [OrganisationId]            VARCHAR(10) NOT NULL,
    [UoPCode]                   VARCHAR(20) NULL,
    [FundingStreamPeriodCode]   VARCHAR(20) NOT NULL,
    [Period]                    INT NOT NULL,
    [DeliverableCode]           INT NOT NULL,
    [ActualVolume]              INT NOT NULL,
    [ActualValue]               DECIMAL(13,2) NOT NULL,
    [PeriodTypeCode]            VARCHAR(10) NOT NULL,
    [ContractAllocationNumber]  VARCHAR(20) NULL,
)