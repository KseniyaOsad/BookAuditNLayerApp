CREATE PROCEDURE [dbo].[sp_GetLastBookAndUserReservation]
	@bookId int,
	@userId int
AS
	SET NOCOUNT ON;

	SELECT TOP(1) Id, BookId, UserId, ReservationDate, ReturnDate FROM [dbo].[Reservations] 
				WHERE BookId = @bookId AND UserId = @userId 
				ORDER BY Id DESC