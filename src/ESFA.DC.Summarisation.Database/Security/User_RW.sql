CREATE USER [DCSummarisation_RW_User]
    WITH PASSWORD = N'$(RWUserPassword)';
GO
	GRANT CONNECT TO [DCSummarisation_RW_User]
GO


