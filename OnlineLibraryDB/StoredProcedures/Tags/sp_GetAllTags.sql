CREATE PROCEDURE [dbo].[sp_GetAllTags]
AS
	SET NOCOUNT ON;

	SELECT T.Id, T.Name, B.Id, B.Name, B.Description, B.Genre , B.InArchive , B.RegistrationDate, BT.TagsId, BT.BooksId
                FROM [dbo].[Tags] AS T 
                LEFT JOIN [dbo].[BookTag]  AS BT ON T.Id = BT.TagsId
                LEFT JOIN [dbo].[Books]    AS B  ON B.Id = BT.BooksId;