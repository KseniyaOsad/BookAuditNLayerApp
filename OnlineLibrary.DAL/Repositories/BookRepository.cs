using AutoMapper;
using Dapper;
using DapperParameters;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using OnlineLibrary.Common.Connection;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.EntityProcessing;
using OnlineLibrary.Common.EntityProcessing.Pagination;
using OnlineLibrary.DAL.Comparer;
using OnlineLibrary.DAL.DTO;
using OnlineLibrary.DAL.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace OnlineLibrary.DAL.Repositories.Dapper
{
    public class BookRepository : IBookRepository
    {
        private readonly string _connectionString;
        
        private readonly IMapper _mapper;

        public BookRepository(IOptions<DBConnection> connOptions, IMapper mapper)
        {
            _connectionString = connOptions.Value.BookContext;
            _mapper = mapper;
        }

        public async Task<PaginatedList<Book>> FilterSortPaginBooksAsync(BookProcessing bookProcessing)
        {
            IEnumerable<Book> books;
            PaginatedList<Book> result = new PaginatedList<Book>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@name", bookProcessing.Filtration.Name);
                parameters.Add("@authorId", bookProcessing.Filtration.AuthorId);
                parameters.Add("@tagId", bookProcessing.Filtration.TagId);
                parameters.Add("@inArchive", bookProcessing.Filtration.Archievation);
                parameters.Add("@sortDirection", bookProcessing.Sorting.SortDirection);
                parameters.Add("@sortProperty", bookProcessing.Sorting.PropertyToOrder);
                parameters.Add("@pageNumber", bookProcessing.Pagination.PageNumber);
                parameters.Add("@pageSize", bookProcessing.Pagination.PageSize);
                parameters.Add("@totalCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

                books = await connection.QueryAsync<Book, Tag, Author, Book>("sp_GetFilterSortPaginBooks", (book, tag, author) =>
                {
                    book.Authors = author == null ? null : new List<Author>() { author };
                    book.Tags = tag == null ? null : new List<Tag>() { tag };
                    return book;
                },
                parameters,
                commandType: CommandType.StoredProcedure);

                result.TotalCount = parameters.Get<int>("@totalCount");
            }
            result.Results = books
                .GroupBy(b => b.Id)
                .Select(group =>
                {
                    var book = group.First();
                    book.Authors = group.Where(b => b.Authors != null).Select(b => b.Authors?.Single()).Distinct(new EntityComparer<Author>()).ToList() ?? new List<Author>();
                    book.Tags = group.Where(b => b.Tags != null).Select(b => b.Tags.Single()).Distinct(new EntityComparer<Tag>()).ToList() ?? new List<Tag>();
                    return book;
                })
                .ToList();
            return result;
        }

        public async Task<List<Book>> GetAllBooksForCsvAsync()
        {
            IEnumerable<Book> books;
            using (var connection = new SqlConnection(_connectionString))
            {
                books = await connection.QueryAsync<Book, Tag, Author, Book>("sp_GetAllBooksForCSV", (book, tag, author) =>
                {
                    book.Authors = author == null ? null : new List<Author>() { author };
                    book.Tags = tag == null ? null : new List<Tag>() { tag };
                    return book;
                },
                commandType: CommandType.StoredProcedure);
            }
            if (!books.Any()) return new List<Book>();
            return books
                .GroupBy(b => b.Id)
                .Select(group =>
                {
                    var book = group.First();
                    book.Authors = group.Where(b => b.Authors != null).Select(b => b.Authors?.Single()).Distinct(new EntityComparer<Author>()).ToList() ?? new List<Author>();
                    book.Tags = group.Where(b => b.Tags != null).Select(b => b.Tags.Single()).Distinct(new EntityComparer<Tag>()).ToList() ?? new List<Tag>();
                    return book;
                })
                .ToList();
        }

        public async Task CreateBookAsync(Book book)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                List<AuthorBookId> authorBookIds = book.Authors.Select(a => new AuthorBookId(a.Id)).ToList();
                List<BookTagId> bookTagIds = book.Tags.Select(t => new BookTagId(t.Id)).ToList();
                var parameters = new DynamicParameters();
                parameters.AddTable("@authorBook", "t_AuthorBook", authorBookIds);
                parameters.AddTable("@bookTag", "t_BookTag", bookTagIds);
                parameters.Add("@name", book.Name);
                parameters.Add("@description", book.Description);
                parameters.Add("@genre", book.Genre);

                book.Id = await connection.ExecuteScalarAsync<int>("sp_CreateBook",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateBookAsync(BookDTO book)
        {
            List<BookTagId> bookTags = book.Tags.Select(tag => new BookTagId(book.Id, tag.Id)).ToList();
            List<AuthorBookId> authorBooks = book.Authors.Select(author => new AuthorBookId(book.Id, author.Id)).ToList();
            List<ReservationDTO> reservations = book.Reservations.Select(r => _mapper.Map<Reservation, ReservationDTO>(r)).ToList();
            var parameters = new DynamicParameters();
            parameters.AddTable("@reservations", "t_Reservation", reservations);
            parameters.AddTable("@bookTag", "t_BookTag", bookTags);
            parameters.AddTable("@authorBook", "t_AuthorBook", authorBooks);
            parameters.Add("@name", book.Name);
            parameters.Add("@description", book.Description);
            parameters.Add("@inArchive", book.InArchive);
            parameters.Add("@genre", book.Genre);
            parameters.Add("@bookId", book.Id);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync("sp_UpdateBook",
                                parameters,
                                commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<Book> GetBookByIdAsync(int bookId)
        {
            IEnumerable<Book> bookGroup;
            using (var connection = new SqlConnection(_connectionString))
            {
                bookGroup = await connection.QueryAsync<Book, Tag, Author, Reservation, User, Book>("sp_GetBookById", (book, tag, author, reservation, user) =>
                {
                    if (reservation != null)
                        reservation.User = user;
                    book.Reservations = reservation == null ? null : new List<Reservation>() { reservation };
                    book.Authors = author == null ? null : new List<Author>() { author };
                    book.Tags = tag == null ? null : new List<Tag>() { tag };
                    return book;
                }, new { id = bookId },
                commandType: CommandType.StoredProcedure);
            }
            if (!bookGroup.Any()) return null;
            Book book = bookGroup.First();
            book.Reservations = bookGroup.Where(b => b.Reservations != null).Select(b => b.Reservations?.Single()).Distinct(new EntityComparer<Reservation>()).ToList() ?? new List<Reservation>();
            book.Authors = bookGroup.Where(b => b.Authors != null).Select(b => b.Authors?.Single()).Distinct(new EntityComparer<Author>()).ToList() ?? new List<Author>();
            book.Tags = bookGroup.Where(b => b.Tags != null).Select(b => b.Tags.Single()).Distinct(new EntityComparer<Tag>()).ToList() ?? new List<Tag>();
            return book;
        }

    }
}
