CREATE PROCEDURE [dbo].[sp_GetBookReservations]
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

    FROM [dbo].[Reservations] AS Res
    LEFT JOIN [dbo].[Users] AS U   ON U.Id = Res.UserId
    LEFT JOIN [dbo].[Books] AS B   ON B.Id = Res.BookId
    WHERE Res.BookId = @bookId
    ORDER BY Res.Id DESC;