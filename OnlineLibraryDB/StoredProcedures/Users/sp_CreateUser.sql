CREATE PROCEDURE [dbo].[sp_CreateUser]
	@name NVARCHAR(100),
	@email NVARCHAR(100),
	@dateOfBirth DATETIME2
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO [dbo].[Users] (Name, Email, DateOfBirth) VALUES (@name, @email, @dateOfBirth);
	SELECT SCOPE_IDENTITY();
END
