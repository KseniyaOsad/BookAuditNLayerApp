CREATE TYPE [dbo].[t_BookTag] AS TABLE
(
	Id INT,
	BookId INT NOT NULL,
	TagId INT NOT NULL
)
