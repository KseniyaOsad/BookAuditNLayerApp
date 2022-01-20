using OnlineLibrary.Common.Entities;
using System;
using System.Collections.Generic;

namespace OnlineLibrary.API.Model
{
    public class CreateBook
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Genre Genre { get; set; }

        public List<int> Tags { get; set; }

        public List<int> Authors { get; set; }

    }
}
