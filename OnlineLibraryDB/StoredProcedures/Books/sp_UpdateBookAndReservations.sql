CREATE PROCEDURE [dbo].[sp_UpdateBookAndReservations]
	@reservations [dbo].[t_Reservation] READONLY,
	@name nvarchar(250),
	@description nvarchar(MAX),
	@inArchive bit,
	@genre tinyint,
	@bookId int
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRAN

		UPDATE [dbo].[Books] 
		SET Name = @name, Description=@description, InArchive=@inArchive, Genre=@genre 
		WHERE Id = @bookId;

		IF NOT EXISTS (SELECT 1 FROM @reservations)
		BEGIN 
			DELETE FROM [dbo].[Reservations] WHERE BookId = @bookId;
		END
		ELSE
		BEGIN
			-- delete all rows where Id not in select
			DELETE FROM [dbo].[Reservations] 
			WHERE Id NOT IN (SELECT Id FROM @reservations) AND BookId = @bookId;

			-- update all reservations
			UPDATE [dbo].[Reservations] 
			SET ReservationDate = RES.ReservationDate, ReturnDate = RES.ReturnDate, UserId = RES.UserId
			FROM @reservations AS RES
			WHERE  
				RES.Id IS NOT NULL
				AND [dbo].[Reservations].Id = RES.Id 
				AND [dbo].[Reservations].BookId = RES.BookId;
			
			--create new reservations
			INSERT INTO [dbo].[Reservations] (BookId, UserId, ReservationDate, ReturnDate)  
			SELECT BookId, UserId, ReservationDate, ReturnDate 
			FROM @reservations 
			WHERE Id = 0 OR Id IS NULL;
		END
	COMMIT; 
END