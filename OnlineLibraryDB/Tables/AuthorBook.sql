CREATE TABLE [dbo].[AuthorBook]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
    [BooksId] INT NOT NULL, 
    [AuthorsId] INT NOT NULL, 
    CONSTRAINT [FK_AuthorBook_Authors_AuthorsId] FOREIGN KEY ([AuthorsId]) REFERENCES [Authors]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AuthorBook_Books_BooksId] FOREIGN KEY ([BooksId]) REFERENCES [Books]([Id]) ON DELETE CASCADE
)
