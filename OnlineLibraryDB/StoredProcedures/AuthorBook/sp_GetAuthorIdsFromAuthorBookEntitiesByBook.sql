CREATE PROCEDURE [dbo].[sp_GetAuthorIdsFromAuthorBookEntitiesByBook]
	@bookId int
AS
	SELECT AuthorsId 
	FROM [dbo].[AuthorBook]
	WHERE BooksId = @bookId;