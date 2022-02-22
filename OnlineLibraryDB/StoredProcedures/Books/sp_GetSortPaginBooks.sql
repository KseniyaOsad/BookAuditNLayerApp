CREATE PROCEDURE [dbo].[sp_GetSortPaginBooks]
    @bookIds [dbo].[t_IdList] READONLY,
    @fromBooks bit = 0,
    @sortDirection nvarchar(100),
    @sortProperty nvarchar(100),
    @skip int,
	@pageSize int
AS
BEGIN 
    SET NOCOUNT ON;

    IF @fromBooks = 0 AND NOT EXISTS(SELECT * FROM @bookIds)
        SET @fromBooks = 1;

    SET @sortProperty  = LOWER(@sortProperty);
    SET @sortDirection  = LOWER(@sortDirection);

    SELECT B.Id, B.Name, B.Description, B.InArchive, B.Genre, B.RegistrationDate, T.Id, T.Name, A.Id, A.Name
                    FROM 
                    (SELECT B.Id, B.Name, B.Description, B.InArchive, B.Genre, B.RegistrationDate 
                        FROM [dbo].[Books] AS B
                        WHERE 
                        (@fromBooks = 1 OR  B.Id IN (SELECT Id FROM @bookIds))
                        ORDER BY 
                        CASE WHEN @sortProperty = 'name' AND @sortDirection = 'asc' THEN B.Name END ASC,
                        CASE WHEN @sortProperty = 'name' AND @sortDirection = 'desc' THEN B.Name END DESC,
                        CASE WHEN @sortProperty = 'genre' AND @sortDirection = 'asc' THEN B.Genre END ASC,
                        CASE WHEN @sortProperty = 'genre' AND @sortDirection = 'desc' THEN B.Genre END DESC,
                        CASE WHEN @sortProperty = 'registrationdate' AND @sortDirection = 'asc' THEN B.RegistrationDate END ASC,
                        CASE WHEN @sortProperty = 'registrationdate' AND @sortDirection = 'desc' THEN B.RegistrationDate END DESC,
                        CASE WHEN @sortProperty = 'id' AND @sortDirection = 'asc' THEN B.Id END ASC,
                        CASE WHEN @sortProperty = 'id' AND @sortDirection = 'desc' THEN B.Id END DESC
                        OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY
                    ) AS B
                    LEFT JOIN [dbo].[AuthorBook] AS AB
                            ON B.Id = AB.BooksId
                            LEFT JOIN [dbo].[Authors] AS A
                                ON AB.AuthorsId = A.Id
                    LEFT JOIN [dbo].[BookTag] AS BT
                            ON B.Id = BT.BooksId
                            LEFT JOIN [dbo].[Tags] AS T
                                ON BT.TagsId = T.Id 

END