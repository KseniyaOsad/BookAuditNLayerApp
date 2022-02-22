CREATE PROCEDURE [dbo].[sp_CreateBookTag]
	@bookTag [dbo].[t_BookTag] READONLY
AS
	SET NOCOUNT ON;

	INSERT INTO [dbo].[BookTag] (BooksId, TagsId) 
	SELECT BookId AS BooksId, TagId AS TagsId 
	FROM @bookTag;