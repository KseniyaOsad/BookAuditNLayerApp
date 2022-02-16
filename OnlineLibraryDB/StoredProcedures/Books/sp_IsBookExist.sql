CREATE PROCEDURE [dbo].[sp_IsBookExist]
	@bookId int
AS
	IF EXISTS   (SELECT 1 
				FROM [dbo].[Books] 
				WHERE Id = @bookId) 
		RETURN 1
	ELSE
		RETURN 0
