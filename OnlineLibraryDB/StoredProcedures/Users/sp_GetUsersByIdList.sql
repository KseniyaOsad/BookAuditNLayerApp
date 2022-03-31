CREATE PROCEDURE [dbo].[sp_GetUsersByIdList]
	@ids [dbo].[t_IdList] READONLY
AS
	SET NOCOUNT ON;

	SELECT Id, Name FROM [dbo].[Users] WHERE Id IN (SELECT Id FROM @ids);
