CREATE TYPE [dbo].[t_AuthorBook] AS TABLE
(
	Id INT,
	BookId INT NOT NULL,
	AuthorId INT NOT NULL
)
