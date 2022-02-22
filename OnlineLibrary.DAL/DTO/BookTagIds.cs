namespace OnlineLibrary.DAL.DTO
{
    class BookTagIds
    {
        public BookTagIds(int bookId, int tagId)
        {
            BookId = bookId;
            TagId = tagId;
        }

        public int BookId { get; set; }

        public int TagId { get; set; }
    }
}
