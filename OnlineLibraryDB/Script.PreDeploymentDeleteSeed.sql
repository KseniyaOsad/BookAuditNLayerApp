﻿DELETE FROM Books;
DELETE FROM Authors;
DELETE FROM Tags;
DELETE FROM Users;

DBCC CHECKIDENT (Books, RESEED, 0);
DBCC CHECKIDENT (Authors, RESEED, 0);
DBCC CHECKIDENT (Tags, RESEED, 0);
DBCC CHECKIDENT (Users, RESEED, 0);

DBCC CHECKIDENT (BookTag, RESEED, 0);
DBCC CHECKIDENT (AuthorBook, RESEED, 0);
DBCC CHECKIDENT (Reservations, RESEED, 0);