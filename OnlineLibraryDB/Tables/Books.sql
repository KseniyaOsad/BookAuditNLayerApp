CREATE TABLE [dbo].[Books]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(250) NOT NULL, 
    [Description] TEXT NOT NULL, 
    [InArchive] BIT NOT NULL DEFAULT 0, 
    [Genre] TINYINT NOT NULL, 
    [RegistrationDate] DATETIME2 NOT NULL DEFAULT GETDATE()
)
