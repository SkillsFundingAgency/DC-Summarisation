CREATE USER [SummarisedActuals_RW_User]
    WITH PASSWORD = N'$(RWUserPassword)';
GO
	GRANT CONNECT TO [SummarisedActuals_RW_User]
GO


