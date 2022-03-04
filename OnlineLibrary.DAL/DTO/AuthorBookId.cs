namespace OnlineLibrary.DAL.DTO
{
    class AuthorBookId
    {
        public AuthorBookId(int authorId)
        {
            AuthorId = authorId;
        }
        public AuthorBookId(int bookId, int authorId)
        {
            BookId = bookId;
            AuthorId = authorId;
        }

        public int Id { get; set; }
        
        public int BookId { get; set; }

        public int AuthorId { get; set; }
    }
}
