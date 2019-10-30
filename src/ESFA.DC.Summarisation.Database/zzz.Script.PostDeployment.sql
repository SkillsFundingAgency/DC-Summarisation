/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

SET NOCOUNT ON ;
GO

RAISERROR('		  Clean Up not used objects',10,1) WITH NOWAIT;

GO
-- Set ExtendedProperties fro DB.
	:r .\z.ExtendedProperties.sql
	
GO
RAISERROR('		   Update User Account Passwords',10,1) WITH NOWAIT;
GO
RAISERROR('		       RO User',10,1) WITH NOWAIT;
ALTER USER [SummarisedActuals_RO_User] WITH PASSWORD = N'$(ROUserPassword)';
GO
RAISERROR('		       RW User',10,1) WITH NOWAIT;
ALTER USER [SummarisedActuals_RW_User] WITH PASSWORD = N'$(RWUserPassword)';
GO
GO
RAISERROR('		       DSCI User',10,1) WITH NOWAIT;
ALTER USER [User_DSCI] WITH PASSWORD = N'$(DSCIUserPassword)';
GO

REVOKE REFERENCES ON SCHEMA::[dbo] FROM [DataProcessing];
REVOKE REFERENCES ON SCHEMA::[dbo] FROM [DataViewing];
GO
DROP USER IF EXISTS [DCSummarisation_RO_User]  
GO
DROP USER IF EXISTS [DCSummarisation_RW_User]  
GO
DROP TABLE IF EXISTS [dbo].[ESF_FundingData]
GO
RAISERROR('Completed',10,1) WITH NOWAIT;
GO
