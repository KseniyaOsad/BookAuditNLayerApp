CREATE PROCEDURE [dbo].[sp_GetLastBookReservation]
	@bookId int
AS
	SET NOCOUNT ON;

	SELECT TOP(1) Id, BookId, UserId, ReservationDate, ReturnDate FROM [dbo].[Reservations] 
				WHERE BookId = @bookId
				ORDER BY ReservationDate DESC