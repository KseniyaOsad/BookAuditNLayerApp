using System;
using System.Collections.Generic;

namespace OnlineLibrary.API.Model
{
    public class CreateBook
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int? Genre { get; set; }

        public List<int> Tags { get; set; }

        public List<int> Authors { get; set; }

        public bool Reserve { get; set; } = false;

        public bool InArchive { get; set; } = false;

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

    }
}
