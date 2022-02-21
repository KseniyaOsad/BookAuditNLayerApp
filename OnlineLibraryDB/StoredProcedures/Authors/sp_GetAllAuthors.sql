CREATE PROCEDURE [dbo].[sp_GetAllAuthors]
AS
	SET NOCOUNT ON;

    SELECT A.Id, A.Name, B.Id, B.Name, B.Description, B.Genre , B.InArchive , B.RegistrationDate, AB.AuthorsId, AB.BooksId
                FROM [dbo].[Authors] AS A
                LEFT JOIN [dbo].[AuthorBook]  AS AB ON A.Id = AB.AuthorsId
                LEFT JOIN [dbo].[Books]       AS B  ON B.Id = AB.BooksId;