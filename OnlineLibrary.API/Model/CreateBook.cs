using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLibrary.API.Model
{
    public class CreateBook
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Genre { get; set; }

        public List<int> Tags { get; set; }

        public List<int> Authors { get; set; }

    }
}
