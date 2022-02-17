CREATE PROCEDURE [dbo].[sp_GetUserById]
	@userId int
AS
    SET NOCOUNT ON;
	SELECT Id, Name FROM [dbo].[Authors] WHERE Id = @userId;
