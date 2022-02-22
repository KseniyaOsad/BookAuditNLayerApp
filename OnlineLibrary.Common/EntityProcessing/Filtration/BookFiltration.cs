
namespace OnlineLibrary.Common.EntityProcessing.Filtration
{
    public class BookFiltration
    {
        private string name;

        public string Name
        {
            get => name;
            set => name = string.IsNullOrWhiteSpace(value) ? null : value;
        }

        private int? authorId;

        public int? AuthorId
        {
            get => authorId;
            set => authorId = value < 1 ? null : value;
        }

        private int? tagId;

        public int? TagId
        {
            get => tagId;
            set => tagId = value < 1 ? null : value;
        }

        private int? archievation;

        public int? Archievation
        {
            get => archievation;
            set => archievation = value < 0 || value > 1 ? null : value;
        }

        public bool CheckFilterPropsNotNull()
        {
            return !string.IsNullOrWhiteSpace(Name) || AuthorId != null || TagId != null || Archievation != null;
        }
    }
}
