CREATE PROCEDURE [dbo].[sp_GetBookById]
	@bookId int
AS
    SET NOCOUNT ON;
	SELECT B.Id, B.Name, B.Description, B.InArchive, B.Genre, B.RegistrationDate ,T.Id, T.Name, A.Id, A.Name
                    FROM [dbo].[Books] AS B
                    LEFT JOIN [dbo].[AuthorBook] AS AB
                            ON B.Id = AB.BooksId
                            LEFT JOIN [dbo].[Authors] AS A
                                ON AB.AuthorsId = A.Id
                    LEFT JOIN [dbo].[BookTag] AS BT
                            ON B.Id = BT.BooksId
                            LEFT JOIN [dbo].[Tags] AS T
                                ON BT.TagsId = T.Id 
                    WHERE B.Id = @bookId;
