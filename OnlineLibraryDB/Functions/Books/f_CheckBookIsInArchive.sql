CREATE FUNCTION [dbo].[f_CheckBookIsInArchive]
(
	@bookId int
)
RETURNS bit
AS
BEGIN
	RETURN (SELECT InArchive FROM [dbo].[Books] WHERE Id = @bookId)
END
