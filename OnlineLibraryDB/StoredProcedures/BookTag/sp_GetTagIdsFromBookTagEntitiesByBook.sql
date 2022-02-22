CREATE PROCEDURE [dbo].[sp_GetTagIdsFromBookTagEntitiesByBook]
	@bookId int
AS
	SELECT TagsId 
	FROM [dbo].[BookTag]
	WHERE BooksId = @bookId;
