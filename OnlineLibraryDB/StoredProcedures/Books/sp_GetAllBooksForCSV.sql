CREATE PROCEDURE [dbo].[sp_GetAllBooksForCSV]
AS
    SET NOCOUNT ON;
	SELECT B.Id, B.Name, T.Id, T.Name, A.Id, A.Name
                    FROM [dbo].[Books] AS B
                    LEFT JOIN [dbo].[AuthorBook] AS AB
                            ON B.Id = AB.BooksId
                            LEFT JOIN [dbo].[Authors] AS A
                                ON AB.AuthorsId = A.Id
                    LEFT JOIN [dbo].[BookTag] AS BT
                            ON B.Id = BT.BooksId
                            LEFT JOIN [dbo].[Tags] AS T
                                ON BT.TagsId = T.Id; 
