SET IDENTITY_INSERT AuthorBook ON 
INSERT INTO AuthorBook (Id, BooksId, AuthorsId) 
VALUES (1, 1, 1), (2, 1, 2), (3, 2, 3);
SET IDENTITY_INSERT AuthorBook OFF;