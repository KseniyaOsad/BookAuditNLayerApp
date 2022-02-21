CREATE PROCEDURE [dbo].[sp_DeleteBookTag]
	@bookId int,
	@tagId int
AS
	SET NOCOUNT ON;
	DELETE FROM [dbo].[BookTag] WHERE BooksId = @bookId AND TagsId = @tagId;