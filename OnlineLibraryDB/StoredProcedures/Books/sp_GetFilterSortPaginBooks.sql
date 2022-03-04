CREATE PROCEDURE [dbo].[sp_GetFilterSortPaginBooks]
    @name nvarchar(250) = NULL,
	@authorId int  = NULL,
    @tagId int  = NULL,
    @inArchive bit  = NULL,

    @sortDirection nvarchar(100),
    @sortProperty nvarchar(100),
    
    @pageNumber int,
	@pageSize int,

    @totalCount int OUTPUT
AS
BEGIN 
    SET NOCOUNT ON;

    SET @sortProperty  = LOWER(@sortProperty);
    SET @sortDirection  = LOWER(@sortDirection);

    DECLARE @ids AS [dbo].[t_IdList];
    INSERT INTO @ids SELECT DISTINCT B.Id AS Id
                                        FROM [dbo].[Books] AS B
                                        LEFT JOIN [dbo].[AuthorBook] AS AB
                                                ON B.Id = AB.BooksId
                                        LEFT JOIN [dbo].[BookTag] AS BT
                                                ON B.Id = BT.BooksId
                                        WHERE 
                                            (@name IS NULL OR B.Name LIKE '%' + @name + '%')
                                            AND
                                            (@inArchive IS NULL OR B.InArchive = @inArchive)
                                            AND
                                            (@tagId IS NULL OR BT.TagsId = @tagId)
                                            AND
                                            (@authorId IS NULL OR AB.AuthorsId = @authorId);
    SET @totalCount = (SELECT COUNT(*) FROM @ids) ;
    SELECT B.Id, B.Name, B.Description, B.InArchive, B.Genre, B.RegistrationDate, T.Id, T.Name, A.Id, A.Name
                    FROM 
                    (SELECT B.Id, B.Name, B.Description, B.InArchive, B.Genre, B.RegistrationDate 
                        FROM [dbo].[Books] AS B
                        WHERE 
                            B.Id IN (SELECT Id FROM @ids)
                        ORDER BY 
                        CASE WHEN @sortProperty = 'name' AND @sortDirection = 'asc' THEN B.Name END ASC,
                        CASE WHEN @sortProperty = 'name' AND @sortDirection = 'desc' THEN B.Name END DESC,
                        CASE WHEN @sortProperty = 'genre' AND @sortDirection = 'asc' THEN B.Genre END ASC,
                        CASE WHEN @sortProperty = 'genre' AND @sortDirection = 'desc' THEN B.Genre END DESC,
                        CASE WHEN @sortProperty = 'registrationdate' AND @sortDirection = 'asc' THEN B.RegistrationDate END ASC,
                        CASE WHEN @sortProperty = 'registrationdate' AND @sortDirection = 'desc' THEN B.RegistrationDate END DESC,
                        CASE WHEN @sortProperty = 'id' AND @sortDirection = 'asc' THEN B.Id END ASC,
                        CASE WHEN @sortProperty = 'id' AND @sortDirection = 'desc' THEN B.Id END DESC
                        OFFSET ((@pageNumber - 1) * @pageSize) ROWS FETCH NEXT @pageSize ROWS ONLY
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