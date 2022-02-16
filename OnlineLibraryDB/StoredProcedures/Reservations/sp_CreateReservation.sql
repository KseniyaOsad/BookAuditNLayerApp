CREATE PROCEDURE [dbo].[sp_CreateReservation]
	@userId INT,
	@bookId INT
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Reservations] (UserId, BookId) 
	VALUES (@userId, @bookId);
END
