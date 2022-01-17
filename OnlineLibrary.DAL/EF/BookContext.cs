using OnlineLibrary.Common.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibrary.DAL.EF
{
    public class BookContext : DbContext
    {
        public DbSet<Book> Book { get; set; }

        public DbSet<Author> Author { get; set; }

        public DbSet<Tag> Tag { get; set; }

        public BookContext(DbContextOptions<BookContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasMany(p => p.Tags)
                .WithMany(p => p.Books);
            modelBuilder.Entity<Book>()
                .HasMany(p => p.Authors)
                .WithMany(p => p.Books);
        }
    }

  
}