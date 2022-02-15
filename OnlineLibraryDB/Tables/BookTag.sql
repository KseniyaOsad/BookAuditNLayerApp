﻿CREATE TABLE [dbo].[BookTag]
(
	[Id] INT IDENTITY (1, 1) PRIMARY KEY, 
    [BooksId] INT NOT NULL, 
    [TagsId] INT NOT NULL, 
    CONSTRAINT [FK_BookTag_Books_BooksId] FOREIGN KEY ([BooksId]) REFERENCES [Books]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_BookTag_Tags_TagsId] FOREIGN KEY ([TagsId]) REFERENCES [Tags]([Id]) ON DELETE CASCADE
)
