CREATE PROCEDURE [dbo].[sp_GetBookById]
	@id int
AS
    SET NOCOUNT ON;
    SELECT 
            B.Id, 
            B.Name, 
            B.Description, 
            B.Genre , 
            B.InArchive , 
            B.RegistrationDate, 

            T.Id, 
            T.Name, 

            A.Id, 
            A.Name,
            
            Res.Id, 
            Res.UserId, 
            Res.BookId, 
            Res.ReturnDate, 
            Res.ReservationDate, 
            
            U.Id, 
            U.Name, 
            U.Email, 
            U.RegistrationDate, 
            U.DateOfBirth 

    FROM [dbo].[Books] AS B

    LEFT JOIN [dbo].[Reservations] AS Res ON B.Id = Res.BookId
    LEFT JOIN [dbo].[Users]        AS U   ON U.Id = Res.UserId

    LEFT JOIN [dbo].[AuthorBook]   AS AB  ON B.Id = AB.BooksId
    LEFT JOIN [dbo].[Authors]      AS A   ON A.Id = AB.AuthorsId

    LEFT JOIN [dbo].[BookTag]      AS BT  ON B.Id = BT.BooksId
    LEFT JOIN [dbo].[Tags]         AS T   ON T.Id = BT.TagsId 

    WHERE B.Id = @id
    ORDER BY Res.Id DESC;