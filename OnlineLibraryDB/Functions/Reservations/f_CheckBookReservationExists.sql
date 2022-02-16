CREATE FUNCTION [dbo].[f_CheckBookReservationExists]
(
	@bookId int
)
RETURNS bit
AS 
BEGIN
	DECLARE @result bit

	SET @result = (
		SELECT CASE 
		WHEN EXISTS (
			SELECT TOP 1 1 
			FROM [dbo].[Reservations]  WHERE BookId = @bookId ORDER BY Id DESC 
		) 
		THEN 1
		ELSE 0
		END);

	RETURN @result
END