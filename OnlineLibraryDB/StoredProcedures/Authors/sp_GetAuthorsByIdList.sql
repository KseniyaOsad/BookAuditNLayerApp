CREATE PROCEDURE [dbo].[sp_GetAuthorsByIdList]
	@ids [dbo].[Id-List] READONLY
AS
	SET NOCOUNT ON;

	SELECT Id, Name FROM [dbo].[Authors] WHERE Id IN (SELECT id FROM @ids);