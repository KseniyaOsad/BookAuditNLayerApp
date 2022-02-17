CREATE PROCEDURE [dbo].[sp_GetAuthorsByIdList]
	@ids [dbo].[IdList] READONLY
AS
	SET NOCOUNT ON;

	SELECT Id, Name FROM [dbo].[Authors] WHERE Id IN (SELECT id FROM @ids);