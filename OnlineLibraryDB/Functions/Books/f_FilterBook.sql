CREATE FUNCTION [dbo].[f_FilterBook]
(
	@name nvarchar(250) = NULL,
	@authorId int  = NULL,
    @tagId int  = NULL,
    @inArchive bit  = NULL
)
RETURNS TABLE
AS
    RETURN SELECT DISTINCT B.Id
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
