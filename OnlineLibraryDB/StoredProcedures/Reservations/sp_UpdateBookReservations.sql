CREATE PROCEDURE [dbo].[sp_UpdateBookReservations]
	@delete [dbo].[t_Reservation] READONLY,
	@update [dbo].[t_Reservation] READONLY,
	@create [dbo].[t_Reservation] READONLY
AS
BEGIN

	BEGIN TRAN
		IF EXISTS (SELECT 1 FROM @delete)
		BEGIN 
			DELETE FROM [dbo].[Reservations] WHERE Id IN (SELECT Id FROM @delete);
		END

		IF EXISTS (SELECT 1 FROM @update)
		BEGIN 
			UPDATE [dbo].[Reservations]
			SET ReservationDate = RES.ReservationDate, ReturnDate = RES.ReturnDate
			FROM (SELECT Id, ReservationDate, ReturnDate FROM @update) AS RES
			WHERE [dbo].[Reservations].Id = RES.Id;
		END

		IF EXISTS (SELECT 1 FROM @create)
		BEGIN 
			INSERT INTO [dbo].[Reservations] (BookId, UserId, ReservationDate, ReturnDate) SELECT BookId, UserId, ReservationDate, ReturnDate FROM @create;
		END
	COMMIT; 
END