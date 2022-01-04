using BookAuditNLayer.GeneralClassLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookAuditNLayerApp.DAL.EF
{
    public class BookContext : DbContext
    {
        public DbSet<Book> Book { get; set; }
        public DbSet<Author> Author { get; set; }
        public BookContext(DbContextOptions<BookContext> options) : base(options)
        {

        }


    }

  
}