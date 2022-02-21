CREATE PROCEDURE [dbo].[sp_GetBookTagEntitiesByBook]
	@bookId int
AS
	SELECT Id, TagsId, BooksId 
	FROM [dbo].[BookTag]
	WHERE BooksId = @bookId;
