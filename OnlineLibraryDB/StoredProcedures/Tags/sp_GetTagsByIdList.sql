CREATE PROCEDURE [dbo].[sp_GetTagsByIdList]
	@ids [dbo].[IdList] READONLY
AS
	SET NOCOUNT ON;

	SELECT Id, Name FROM [dbo].[Tags] WHERE Id IN (SELECT id FROM @ids);