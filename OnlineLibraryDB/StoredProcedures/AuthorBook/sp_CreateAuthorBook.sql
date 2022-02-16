CREATE PROCEDURE [dbo].[sp_CreateAuthorBook]
	@bookId int,
	@authorId int
AS
	SET NOCOUNT ON;
	INSERT INTO [dbo].[AuthorBook] (BooksId, AuthorsId) VALUES (@bookId, @authorId);