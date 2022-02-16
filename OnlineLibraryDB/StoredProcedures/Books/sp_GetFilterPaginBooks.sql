CREATE PROCEDURE [dbo].[sp_GetFilterPaginBooks]
	@name nvarchar(250) = NULL,
	@authorId int  = NULL,
    @tagId int  = NULL,
    @inArchive bit  = NULL,
    @sortDirection nvarchar(100),
    @sortProperty nvarchar(100),
    @pageNumber int,
	@pageSize int
AS

BEGIN 
    SET NOCOUNT ON;

    DECLARE @bookIds [dbo].[Id-List] ;
    INSERT INTO  @bookIds SELECT Id FROM [dbo].[f_FilterBook](@name, @authorId, @tagId, @inArchive);

    DECLARE @bookCount int = (SELECT COUNT(*) FROM @bookIds);
    DECLARE @skip int = [dbo].[f_CalculateSkip](@bookCount, @pageNumber, @pageSize);
    
    SET @sortProperty  = LOWER(@sortProperty);
    SET @sortDirection  = LOWER(@sortDirection);

    SELECT B.Id, B.Name, B.Description, B.InArchive, B.Genre, B.RegistrationDate, T.Id, T.Name, A.Id, A.Name
                    FROM 
                    (SELECT B.Id, B.Name, B.Description, B.InArchive, B.Genre, B.RegistrationDate 
                        FROM [dbo].[Books] AS B
                        WHERE B.Id IN (SELECT Id FROM @bookIds)
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