CREATE PROCEDURE [dbo].[sp_GetBookInfoAndBookReservations]
	@bookId int
AS
	SET NOCOUNT ON;

    SELECT 
            Res.Id, 
            Res.UserId, 
            Res.BookId, 
            Res.ReturnDate, 
            Res.ReservationDate, 
            U.Id, 
            U.Name, 
            U.Email, 
            U.RegistrationDate, 
            U.DateOfBirth, 
            B.Id, 
            B.Name, 
            B.Description, 
            B.Genre , 
            B.InArchive , 
            B.RegistrationDate 
    FROM [dbo].[Books] AS B
    LEFT JOIN [dbo].[Reservations] AS Res ON B.Id = Res.BookId
    LEFT JOIN [dbo].[Users]        AS U   ON U.Id = Res.UserId
    WHERE Res.BookId = @bookId
    ORDER BY Res.Id DESC;