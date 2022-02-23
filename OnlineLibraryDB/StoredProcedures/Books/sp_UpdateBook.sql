CREATE PROCEDURE [dbo].[sp_UpdateBook]
	@reservations [dbo].[t_Reservation] READONLY,
	@authorBook [dbo].[t_AuthorBook] READONLY,
	@bookTag [dbo].[t_BookTag] READONLY,
	@name nvarchar(250) = NULL,
	@description nvarchar(MAX) = NULL,
	@inArchive bit = NULL,
	@genre tinyint = NULL,
	@bookId int
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRAN

		-- update book
		UPDATE [dbo].[Books] 
		SET
			Name = IsNull(@name, Name),
			Description = IsNull(@description, Description),
			InArchive = IsNull(@inArchive, InArchive),
			Genre = IsNull(@genre, Genre)
		WHERE Id = @bookId;
		-- end updating book

		-- update reservation
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
		-- end updating reservation

		-- update authorBook
		IF EXISTS (SELECT 1 FROM @authorBook)
		BEGIN
			-- delete all AuthorBook
			DELETE FROM [dbo].[AuthorBook] 
			WHERE BooksId = @bookId;

			--create new AuthorBook
			INSERT INTO [dbo].[AuthorBook]  (BooksId, AuthorsId)  
			SELECT BookId AS BooksId, AuthorId AS AuthorsId
			FROM @authorBook;
		END
		-- end updating authorBook

		-- update bookTag
		IF EXISTS (SELECT 1 FROM @bookTag)
		BEGIN
			-- delete all bookTag
			DELETE FROM [dbo].[BookTag] 
			WHERE BooksId = @bookId;

			--create new bookTag
			INSERT INTO [dbo].[BookTag]  (BooksId, TagsId)  
			SELECT BookId AS BooksId, TagId AS TagsId
			FROM @bookTag;
		END
		-- end updating bookTag

	COMMIT; 
END