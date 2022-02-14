SET IDENTITY_INSERT Users ON
INSERT INTO Users (Id, Name, Email, DateOfBirth) 
VALUES (1, 'Viktor', 'victor@gmail.com', '1999-01-01'), (2, 'Anya', 'anya@gmail.com', '1989-11-10');
SET IDENTITY_INSERT Users OFF;