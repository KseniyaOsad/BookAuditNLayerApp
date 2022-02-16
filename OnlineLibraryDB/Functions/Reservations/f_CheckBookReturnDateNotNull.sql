CREATE FUNCTION [dbo].[f_CheckBookReturnDateNotNull]
(
	@bookId int
)
RETURNS bit
AS 
BEGIN
	DECLARE @returnDate datetime2
	DECLARE @result bit

	SET @returnDate = (SELECT TOP 1 ReturnDate FROM [dbo].[Reservations] WHERE BookId = @bookId ORDER BY Id DESC )

	IF (@returnDate is null) SET @result = 0
	ELSE SET @result = 1

	RETURN @result
END