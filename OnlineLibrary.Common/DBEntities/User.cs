using System;
using System.Collections.Generic;

namespace OnlineLibrary.Common.DBEntities
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public DateTime DateOfBirth { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}
