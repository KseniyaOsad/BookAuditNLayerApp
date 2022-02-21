CREATE PROCEDURE [dbo].[sp_GetAllUsers]
AS
	SET NOCOUNT ON;

	SELECT Id, Name, Email, RegistrationDate, DateOfBirth FROM [dbo].[Users];
