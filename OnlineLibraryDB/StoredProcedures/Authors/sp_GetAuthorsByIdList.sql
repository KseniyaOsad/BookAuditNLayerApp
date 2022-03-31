CREATE PROCEDURE [dbo].[sp_GetAuthorsByIdList]
	@ids [dbo].[t_IdList] READONLY
AS
	SET NOCOUNT ON;

	SELECT Id, Name FROM [dbo].[Authors] WHERE Id IN (SELECT Id FROM @ids);