CREATE FUNCTION [dbo].[f_CalculateSkip]
(
	@booksCount int,
	@pageNumber int,
	@pageSize int 
)
RETURNS INT
AS
BEGIN
    -- If we need to show more data than we have, show the first page.
	IF @booksCount < @pageSize RETURN 0;

	-- If after skipping data there is some data, show it.
	DECLARE @skip int = (@pageNumber - 1) * @pageSize;

	IF (@booksCount - @skip >= 1) RETURN @skip;

	-- Else calculate how much we need to skip for obtaining the last page.
	DECLARE @remainder int = @booksCount %  @pageSize;

	IF @remainder = 0 RETURN @booksCount - @pageSize;

	RETURN @booksCount - @remainder; 
END
