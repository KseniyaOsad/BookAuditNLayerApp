CREATE PROCEDURE [dbo].[sp_DeleteAuthorBook]
	@bookId int,
	@authorId int
AS
	SET NOCOUNT ON;
	DELETE FROM [dbo].[AuthorBook] WHERE BooksId = @bookId AND AuthorsId = @authorId;
