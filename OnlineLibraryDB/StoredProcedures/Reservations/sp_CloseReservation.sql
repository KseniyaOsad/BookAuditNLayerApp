CREATE PROCEDURE [dbo].[sp_CloseReservation]
	@bookId int,
	@userId int
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE [dbo].[Reservations] 
	SET ReturnDate = GETUTCDATE() 
	WHERE BookId = @bookId AND UserId = @userId;
END
