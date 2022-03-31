CREATE PROCEDURE [dbo].[sp_CreateBook]
	@name NVARCHAR(250),
	@description NVARCHAR(MAX),
	@genre TINYINT,
	@authorBook [dbo].[t_AuthorBook] READONLY,
	@bookTag [dbo].[t_BookTag] READONLY
AS
BEGIN 
	SET NOCOUNT ON;
	DECLARE @id int;
	BEGIN TRAN
	
	INSERT INTO [dbo].[Books] (Name, Description, Genre)
	VALUES (@name, @description, @genre);
	
	SET @id = (SELECT SCOPE_IDENTITY());
	
	INSERT INTO [dbo].[AuthorBook] (BooksId, AuthorsId)
	SELECT @id AS BooksId, AuthorId AS AuthorsId FROM @authorBook;
	
	INSERT INTO [dbo].[BookTag] (BooksId, TagsId)
	SELECT @id AS BooksId, TagId AS TagsId FROM @bookTag;
	
	COMMIT; 
	SELECT @id;
END
