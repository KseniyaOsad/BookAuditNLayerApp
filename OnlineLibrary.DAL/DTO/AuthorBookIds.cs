namespace OnlineLibrary.DAL.DTO
{
    class AuthorBookIds
    {
        public AuthorBookIds(int bookId, int authorId)
        {
            BookId = bookId;
            AuthorId = authorId;
        }

        public int BookId { get; set; }

        public int AuthorId { get; set; }
    }
}
