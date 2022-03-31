CREATE PROCEDURE [dbo].[sp_GetTagsByIdList]
	@ids [dbo].[t_IdList] READONLY
AS
	SET NOCOUNT ON;

	SELECT Id, Name FROM [dbo].[Tags] WHERE Id IN (SELECT Id FROM @ids);