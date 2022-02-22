CREATE PROCEDURE [dbo].[sp_DeleteAuthorBook]
	@authorBook [dbo].[t_AuthorBook] READONLY
AS
	SET NOCOUNT ON;

	DELETE AB FROM [dbo].[AuthorBook] AS AB
	INNER JOIN @authorBook AS ABList 
	ON ABList.BookId = AB.BooksId
	WHERE ABList.AuthorId = AB.AuthorsId;