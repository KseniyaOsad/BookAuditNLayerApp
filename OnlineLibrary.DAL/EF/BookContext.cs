using OnlineLibrary.Common.DBEntities;
using Microsoft.EntityFrameworkCore;

namespace OnlineLibrary.DAL.EF
{
    public class BookContext : DbContext
    {
        public DbSet<Book> Books { get; set; }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

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
            modelBuilder.Entity<Reservation>()
                .HasOne(p => p.Book)
                .WithMany()
                .HasForeignKey(p=>p.BookId);
            modelBuilder.Entity<Reservation>()
               .HasOne(p => p.User)
               .WithMany()
               .HasForeignKey(p=>p.UserId);
        }
    }
}