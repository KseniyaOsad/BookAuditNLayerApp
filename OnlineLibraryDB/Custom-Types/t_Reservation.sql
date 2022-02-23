CREATE TYPE [dbo].[t_Reservation] AS TABLE
(
	[Id] INT NULL, 
    [UserId] INT NOT NULL, 
    [BookId] INT NOT NULL, 
    [ReservationDate] DATETIME2 NOT NULL, 
    [ReturnDate] DATETIME2 NULL
)
