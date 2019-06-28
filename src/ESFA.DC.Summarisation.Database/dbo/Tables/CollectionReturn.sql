CREATE TABLE [dbo].[CollectionReturn]
(
	[Id]						INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[CollectionType]            VARCHAR(20) NOT NULL,
    [CollectionReturnCode]      VARCHAR(10) NOT NULL,
)
GO
ALTER TABLE [dbo].[CollectionReturn]
ADD CONSTRAINT UK_CollectionReturn UNIQUE (CollectionType,CollectionReturnCode)
GO