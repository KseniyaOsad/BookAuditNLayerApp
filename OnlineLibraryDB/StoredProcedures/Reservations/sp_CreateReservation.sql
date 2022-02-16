CREATE PROCEDURE [dbo].[sp_CreateReservation]
	@userId INT,
	@bookId INT
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY

		IF [dbo].[f_CheckBookIsInArchive] (@bookId) = 1 
		BEGIN
			 SELECT -1 AS NewId, 400 AS Status, 'Book is in archive' AS ExceptionMessage;
			 RETURN;
		END

		IF [dbo].[f_CheckBookReservationExists] (@bookId) = 0 OR [dbo].[f_CheckBookReturnDateNotNull] (@bookId) = 1
			BEGIN 
				INSERT INTO [dbo].[Reservations] (UserId, BookId) 
				VALUES (@userId, @bookId);
				SELECT SCOPE_IDENTITY() AS NewId, 200 AS Status, '' AS ExceptionMessage;
			END 
		ELSE 
			SELECT -1 AS NewId, 400 AS Status, 'Book is in reserve' AS ExceptionMessage;

	END TRY
	BEGIN CATCH 
		SELECT -1 AS NewID, 400 AS Status, 'User or Book not found' AS ExceptionMessage;
	END CATCH
END
