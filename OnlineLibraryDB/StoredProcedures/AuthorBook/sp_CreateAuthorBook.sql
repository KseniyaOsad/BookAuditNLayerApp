CREATE PROCEDURE [dbo].[sp_CreateAuthorBook]
	@authorBook [dbo].[t_AuthorBook] READONLY
AS
	SET NOCOUNT ON;

	INSERT INTO [dbo].[AuthorBook] (BooksId, AuthorsId) 
	SELECT BookId AS BooksId, AuthorId AS AuthorsId
	FROM @authorBook;