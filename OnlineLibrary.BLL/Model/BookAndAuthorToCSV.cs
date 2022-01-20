using System;
using System.Text;

namespace OnlineLibrary.BLL.Model
{
    public class BookAndAuthorToCSV
    {
        public int Id { get; set; }

        private string _title;

        public string Title
        {

            get { return _title; }

            set
            {
                if (value.Contains(','))
                {
                    _title = String.Format("\"{0}\"", value);
                }
                else
                {
                    _title = value;
                }
            }
        }

        public string AuthorName { get; set; }

        public override string ToString()
        {

            return new StringBuilder(Id.ToString()).Append(",").Append(Title).Append(",").Append(AuthorName).Append("\n").ToString();
        }
    }
}
