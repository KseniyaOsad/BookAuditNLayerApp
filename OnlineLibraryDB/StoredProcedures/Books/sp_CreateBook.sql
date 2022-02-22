CREATE PROCEDURE [dbo].[sp_CreateBook]
	@name NVARCHAR(250),
	@description NVARCHAR(MAX),
	@genre TINYINT
AS
BEGIN 
	SET NOCOUNT ON;
	
	INSERT INTO [dbo].[Books] (Name, Description, Genre)
	VALUES (@name, @description, @genre);
	
	SELECT SCOPE_IDENTITY();
END
