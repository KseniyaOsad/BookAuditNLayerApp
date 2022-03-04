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
		MERGE [dbo].[Reservations]  AS tgt  
			USING 
					(SELECT Id ,UserId ,BookId , ReservationDate ,ReturnDate FROM @reservations) 
					as src 
					(Id ,UserId ,BookId ,ReservationDate ,ReturnDate)  
			ON (tgt.Id = src.Id AND tgt.BookId = src.BookId)  
			WHEN MATCHED THEN
				UPDATE SET UserId = src.UserId, ReservationDate = src.ReservationDate, ReturnDate = src.ReturnDate  
			WHEN NOT MATCHED THEN  
				INSERT ( UserId ,BookId ,ReservationDate ,ReturnDate)  
				VALUES (src.UserId ,src.BookId ,src.ReservationDate ,src.ReturnDate)  
		WHEN NOT MATCHED BY SOURCE AND (tgt.BookId = BookId) THEN DELETE;

		-- update authorBook
		MERGE [dbo].[AuthorBook] AS tgt
			USING 
				(SELECT Id, BookId, AuthorId FROM @authorBook) AS src (Id, BooksId, AuthorsId)
			ON (tgt.Id = src.Id AND tgt.BooksId = src.BooksId)
			WHEN MATCHED THEN 
				UPDATE SET AuthorsId = src.AuthorsId
			WHEN NOT MATCHED THEN 
				INSERT (BooksId, AuthorsId) 
				VALUES (src.BooksId, src.AuthorsId)
			WHEN NOT MATCHED BY SOURCE AND (tgt.BooksId = BooksId) THEN
				DELETE;

		-- update bookTag
		MERGE [dbo].[BookTag] AS tgt
			USING 
				(SELECT Id, BookId, TagId FROM @bookTag)
				AS src
				(Id, BooksId, TagsId)
			ON (tgt.Id = src.Id AND tgt.BooksId = src.BooksId)
			WHEN MATCHED THEN 
				UPDATE SET TagsId = src.TagsId 
			WHEN NOT MATCHED THEN 
				INSERT (BooksId, TagsId)
				VALUES (src.BooksId, src.TagsId)
			WHEN NOT MATCHED BY SOURCE AND (tgt.BooksId = BooksId) THEN
				DELETE;
	COMMIT; 
END