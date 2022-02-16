CREATE PROCEDURE [dbo].[sp_CreateBook]
	@name NVARCHAR(250),
	@description TEXT,
	@genre TINYINT
AS
BEGIN 
	SET NOCOUNT ON;
	
	INSERT INTO [dbo].[Books] (Name, Description, Genre)
	VALUES (@name, @description, @genre);
	
	RETURN SCOPE_IDENTITY();
END
