SET IDENTITY_INSERT Reservations ON
INSERT INTO Reservations (Id, BookId, UserId, ReturnDate) 
VALUES (1, 1, 1, '2020-12-02'), (2, 1, 2, '2020-12-02');
INSERT INTO Reservations (Id, BookId, UserId) 
VALUES (3, 2, 1);
SET IDENTITY_INSERT Reservations OFF;