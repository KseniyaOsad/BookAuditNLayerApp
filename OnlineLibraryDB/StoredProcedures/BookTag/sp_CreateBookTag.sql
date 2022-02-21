CREATE PROCEDURE [dbo].[sp_CreateBookTag]
	@bookId int,
	@tagId int
AS
	SET NOCOUNT ON;
	INSERT INTO [dbo].[BookTag] (BooksId, TagsId) VALUES (@bookId, @tagId);