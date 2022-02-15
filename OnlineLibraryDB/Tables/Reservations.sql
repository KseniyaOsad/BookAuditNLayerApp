CREATE TABLE [dbo].[Reservations]
(
	[Id] INT IDENTITY (1, 1) PRIMARY KEY, 
    [UserId] INT NOT NULL, 
    [BookId] INT NOT NULL, 
    [ReservationDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), 
    [ReturnDate] DATETIME2 NULL,
    CONSTRAINT [FK_Reservations_Books_BookId] FOREIGN KEY ([BookId]) REFERENCES [Books]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Reservations_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE
)
