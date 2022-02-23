SET IDENTITY_INSERT Reservations ON
INSERT INTO Reservations (Id, BookId, UserId, ReservationDate, ReturnDate) 
VALUES (1, 1, 1, '2020-11-02', '2020-12-02'), (2, 1, 2, '2020-12-03', '2020-12-22');
INSERT INTO Reservations (Id, BookId, UserId, ReservationDate) 
VALUES (3, 2, 1, '2020-11-02');
SET IDENTITY_INSERT Reservations OFF;