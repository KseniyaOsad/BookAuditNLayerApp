CREATE PROCEDURE [dbo].[sp_GetAllReservations]
AS
	SET NOCOUNT ON;

	SELECT Res.Id, Res.UserId, Res.BookId, Res.ReturnDate, Res.ReservationDate, U.Id, U.Name, U.Email, U.RegistrationDate, U.DateOfBirth, B.Id, B.Name, B.Description, B.Genre , B.InArchive , B.RegistrationDate 
                FROM [dbo].[Reservations] AS Res
                LEFT JOIN [dbo].[Users] AS U   ON U.Id = Res.UserId
                LEFT JOIN [dbo].[Books] AS B   ON B.Id = Res.BookId
                ORDER BY Res.Id DESC;
