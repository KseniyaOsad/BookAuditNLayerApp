namespace OnlineLibrary.DAL.DTO
{
    class BookTagId
    {
        public BookTagId(int tagId)
        {
            TagId = tagId;
        }

        public BookTagId(int bookId, int tagId)
        {
            BookId = bookId;
            TagId = tagId;
        }

        public int Id { get; set; }

        public int BookId { get; set; }

        public int TagId { get; set; }
    }
}
