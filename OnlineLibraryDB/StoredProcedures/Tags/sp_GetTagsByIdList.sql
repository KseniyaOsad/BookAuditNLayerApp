CREATE PROCEDURE [dbo].[sp_GetTagsByIdList]
	@ids [dbo].[Id-List] READONLY
AS
	SET NOCOUNT ON;

	SELECT Id, Name FROM [dbo].[Tags] WHERE Id IN (SELECT id FROM @ids);