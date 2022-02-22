CREATE PROCEDURE [dbo].[sp_UpdateBook]
	@name nvarchar(250),
	@description nvarchar(MAX),
	@inArchive bit,
	@genre tinyint,
	@bookId int
AS
	SET NOCOUNT ON;
	UPDATE [dbo].[Books] 
	SET Name = @name, Description=@description, InArchive=@inArchive, Genre=@genre 
	WHERE Id = @bookId;