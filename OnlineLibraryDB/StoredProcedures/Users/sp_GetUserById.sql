CREATE PROCEDURE [dbo].[sp_GetUserById]
	@id int
AS
    SET NOCOUNT ON;
	SELECT Id, Name FROM [dbo].[Authors] WHERE Id = @id;
