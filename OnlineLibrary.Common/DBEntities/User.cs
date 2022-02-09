﻿using System;

namespace OnlineLibrary.Common.DBEntities
{
    class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public int Age { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}