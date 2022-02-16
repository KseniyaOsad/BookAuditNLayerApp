CREATE PROCEDURE [dbo].[sp_IsUserExist]
	@userId int
AS
	IF EXISTS   (SELECT 1 
				FROM [dbo].[Users] 
				WHERE Id = @userId) 
		RETURN 1
	ELSE
		RETURN 0
