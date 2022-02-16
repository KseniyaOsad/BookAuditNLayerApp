CREATE PROCEDURE [dbo].[sp_CloseReservation]
	@bookId int,
	@userId int
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @row TABLE (
		Id int,
		ReturnDate datetime2
		);

	INSERT @row SELECT TOP(1) Id, ReturnDate FROM [dbo].[Reservations] 
				WHERE BookId = @bookId AND @userId = @userId 
				ORDER BY Id DESC;
	
	
	IF NOT EXISTS   (SELECT TOP(1) 1 FROM @row)
	BEGIN
		SELECT 400 AS Status, 'Reservation not found' AS ExceptionMessage;
		RETURN;
	END

	IF ((SELECT TOP(1) ReturnDate FROM @row) IS NOT NULL) 
	BEGIN
		SELECT 400 AS Status, 'Reservation is already closed' AS ExceptionMessage;
		RETURN;
	END

	UPDATE [dbo].[Reservations] SET ReturnDate =  GETUTCDATE() WHERE Id = (SELECT TOP(1) Id FROM @row);
	SELECT 200 AS Status, '' AS ExceptionMessage;

END
