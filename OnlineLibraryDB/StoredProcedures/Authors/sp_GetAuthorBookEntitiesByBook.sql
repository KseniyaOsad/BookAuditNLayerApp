CREATE PROCEDURE [dbo].[sp_GetAuthorBookEntitiesByBook]
	@bookId int
AS
	SELECT Id, AuthorsId, BooksId 
	FROM [dbo].[AuthorBook]
	WHERE BooksId = @bookId;