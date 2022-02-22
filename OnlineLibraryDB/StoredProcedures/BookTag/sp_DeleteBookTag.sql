CREATE PROCEDURE [dbo].[sp_DeleteBookTag]
	@bookTag [dbo].[t_BookTag] READONLY
AS
	SET NOCOUNT ON;

	DELETE BT FROM [dbo].[BookTag] AS BT
	INNER JOIN @bookTag AS BTList 
	ON BTList.BookId = BT.BooksId
	WHERE BTList.TagId = BT.TagsId;
