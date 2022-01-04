﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BookAuditNLayerApp.BLL.DTO
{
    public class AuthorDTO
    {
        // Планировалось, что будет использоваться в этом проекте в качестве альтернативы для класса Author,
        // но так как он полностью копирует класс Author, он вскоре будет изменен или удален из проекта совсем 
        // Вместо него будет использоваться класс Author, который был перенесен в папку Entities в проекте GeneralClassLibrary.

        public int Id { get; set; }

        public string Name { get; set; }
    }
}