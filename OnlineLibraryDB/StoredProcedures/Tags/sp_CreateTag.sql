CREATE PROCEDURE [dbo].[sp_CreateTag]
	@name NVARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO [dbo].[Tags] (Name) VALUES (@name);
	SELECT SCOPE_IDENTITY();
END