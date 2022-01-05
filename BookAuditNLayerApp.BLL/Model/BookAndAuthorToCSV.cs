using System;
using System.Collections.Generic;
using System.Text;

namespace BookAuditNLayerApp.BLL.Model
{
    public class BookAndAuthorToCSV
    {
        public int Id { get; set; }

        public string Title { 
            
            get{ return Title; } 
            
            set{
                if (value.Contains(','))
                {
                    Title = String.Format("\"{0}\"", value);
                }
                else
                {
                    Title = value;
                }
            } 
        }

        public string AuthorName { get; set; }

        public override string ToString()
        {
            return new StringBuilder(Id.ToString()).Append(",").Append(Title).Append(",").Append(AuthorName).Append("/n").ToString();
        }
    }
}
