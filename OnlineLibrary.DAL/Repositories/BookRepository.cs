﻿using Dapper;
using DapperParameters;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using OnlineLibrary.Common.Connection;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.EntityProcessing.Filtration;
using OnlineLibrary.Common.EntityProcessing.Sorting;
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

        public BookRepository(IOptions<DBConnection> connOptions)
        {
            _connectionString = connOptions.Value.BookContext;
        }

        public async Task<List<int>> FilterBooksAsync(BookFiltration bookFiltration)
        {
            using (var connection = new SqlConnection(_connectionString))
                return (await connection.QueryAsync<int>("sp_GetFilterBookIds", 
                    new {
                        name = bookFiltration.Name,
                        authorId = bookFiltration.AuthorId,
                        tagId = bookFiltration.TagId,
                        inArchive = bookFiltration.Archievation
                    },
                    commandType: CommandType.StoredProcedure)).ToList();
        }

        public async Task<List<Book>> SortPaginBooksAsync(List<int> bookIds, bool fromBooks, SortingOptions sortingOptions, int skip, int pageSize)
        {
            IEnumerable<Book> Books;
            using (var connection = new SqlConnection(_connectionString))
            {
                List<IdList> idLists = bookIds.Select(x => new IdList(x)).ToList();
                var parameters = new DynamicParameters();
                parameters.AddTable("@bookIds", "t_IdList", idLists);
                parameters.Add("@fromBooks", fromBooks);
                parameters.Add("@sortDirection", sortingOptions.SortDirection);
                parameters.Add("@sortProperty", sortingOptions.PropertyToOrder);
                parameters.Add("@skip", skip);
                parameters.Add("@pageSize", pageSize);

                Books = await connection.QueryAsync<Book, Tag, Author, Book>("sp_GetSortPaginBooks", (book, tag, author) =>
                {
                    book.Authors = author == null ? null : new List<Author>() { author };
                    book.Tags = tag == null ? null : new List<Tag>() { tag };
                    return book;
                },
                parameters,
                commandType: CommandType.StoredProcedure);
            }
            if (!Books.Any()) return null;
            return Books
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

        public async Task<List<Book>> GetAllBooksForCsvAsync()
        {
            IEnumerable<Book> Books;
            using (var connection = new SqlConnection(_connectionString))
            {
                Books = await connection.QueryAsync<Book, Tag, Author, Book>("sp_GetAllBooksForCSV", (book, tag, author) =>
                {
                    book.Authors = author == null ? null : new List<Author>() { author };
                    book.Tags = tag == null ? null : new List<Tag>() { tag };
                    return book;
                },
                commandType: CommandType.StoredProcedure);
            }
            if (!Books.Any()) return new List<Book>();
            return Books
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

        public async Task<Book> GetBookByIdAsync(int bookId)
        {
            IEnumerable<Book> BookGroup;
            using (var connection = new SqlConnection(_connectionString))
            {
                BookGroup = await connection.QueryAsync<Book, Tag, Author, Book>("sp_GetBookById", (book, tag, author) =>
                {
                    book.Authors = author == null ? null : new List<Author>() { author };
                    book.Tags = tag == null ? null : new List<Tag>() { tag };
                    return book;
                }, new { id = bookId },
                commandType: CommandType.StoredProcedure);
            }
            if (!BookGroup.Any()) return null;
            Book book = BookGroup.First();
            book.Authors = BookGroup.Where(b => b.Authors != null).Select(b => b.Authors?.Single()).Distinct(new EntityComparer<Author>()).ToList() ?? new List<Author>();
            book.Tags = BookGroup.Where(b => b.Tags != null).Select(b => b.Tags.Single()).Distinct(new EntityComparer<Tag>()).ToList() ?? new List<Tag>();
            return book;
        }

        public async Task CreateBookAsync(Book book)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    book.Id = await connection.ExecuteScalarAsync<int>("sp_CreateBook", 
                        new { name = book.Name, description  = book.Description, genre = book.Genre},
                        commandType: CommandType.StoredProcedure);

                    List<AuthorBookIds> authorBookIds = book.Authors.Select(a => new AuthorBookIds(book.Id, a.Id)).ToList();
                    var parameters = new DynamicParameters();
                    parameters.AddTable("@authorBook", "t_AuthorBook", authorBookIds);

                    await connection.ExecuteAsync("sp_CreateAuthorBook",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    List<BookTagIds> bookTagIds = book.Tags.Select(t => new BookTagIds(book.Id, t.Id)).ToList();
                    parameters = new DynamicParameters();
                    parameters.AddTable("@bookTag", "t_BookTag", bookTagIds);

                    await connection.ExecuteAsync("sp_CreateBookTag",
                        parameters,
                        commandType: CommandType.StoredProcedure);
                    
                    scope.Complete();
                }
            }
        }

        public async Task<List<int>> GetAuthorIdsByBookAsync(int bookId)
        {
            using (var connection = new SqlConnection(_connectionString))
                return (await connection.QueryAsync<int>("sp_GetAuthorIdsFromAuthorBookEntitiesByBook",
                        new { bookId = bookId },
                        commandType: CommandType.StoredProcedure)).ToList();
        }

        public async Task<List<int>> GetTagIdsByBookAsync(int bookId)
        {
            using (var connection = new SqlConnection(_connectionString))
                return (await connection.QueryAsync<int>("sp_GetTagIdsFromBookTagEntitiesByBook",
                        new { bookId = bookId },
                        commandType: CommandType.StoredProcedure)).ToList();
        }

        public async Task UpdateAuthorBookAsync(Book book, List<int> newAuthors, List<int> oldAuthors)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Create new row if needed.
                    if (newAuthors.Any())
                    {
                        List<AuthorBookIds> authorBookIds = newAuthors.Select(authorId => new AuthorBookIds(book.Id, authorId)).ToList();
                        var parameters = new DynamicParameters();
                        parameters.AddTable("@authorBook", "t_AuthorBook", authorBookIds);

                        await connection.ExecuteAsync("sp_CreateAuthorBook",
                                parameters,
                                commandType: CommandType.StoredProcedure);
                    }

                    // Delete old rows if needed.
                    if (oldAuthors.Any())
                    {
                        List<AuthorBookIds> authorBookIds = oldAuthors.Select(authorId => new AuthorBookIds(book.Id, authorId)).ToList();
                        var parameters = new DynamicParameters();
                        parameters.AddTable("@authorBook", "t_AuthorBook", authorBookIds);

                        await connection.ExecuteAsync("sp_DeleteAuthorBook",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                    }
                    scope.Complete();
                }
            }
        }

        public async Task UpdateBookTagAsync(Book book, List<int> newTags, List<int> oldTags)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Create new row if needed.
                    if (newTags.Any())
                    {
                        List<BookTagIds> bookTagIds = newTags.Select(tagId => new BookTagIds(book.Id, tagId)).ToList();
                        var parameters = new DynamicParameters();
                        parameters.AddTable("@bookTag", "t_BookTag", bookTagIds);

                        await connection.ExecuteAsync("sp_CreateBookTag",
                                parameters,
                                commandType: CommandType.StoredProcedure);
                    }

                    // Delete old rows if needed.
                    if (oldTags.Any())
                    {
                        List<BookTagIds> bookTagIds = oldTags.Select(tagId => new BookTagIds(book.Id, tagId)).ToList();
                        var parameters = new DynamicParameters();
                        parameters.AddTable("@bookTag", "t_BookTag", bookTagIds);

                        await connection.ExecuteAsync("sp_DeleteBookTag",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                    }
                    scope.Complete();
                }
            }
        }

        public async Task UpdateBookAsync(Book book)
        {
             using (var connection = new SqlConnection(_connectionString))
             {
                 await connection.ExecuteAsync("sp_UpdateBook", new
                 {
                     name = book.Name,
                     description = book.Description,
                     inArchive = book.InArchive,
                     genre = book.Genre,
                     bookId = book.Id
                 },
                 commandType: CommandType.StoredProcedure);
             }
        }

        public async Task<Book> GetBookInfoAndBookReservationsAsync(int bookId)
        {
            IEnumerable<Book> BookGroup;
            using (var connection = new SqlConnection(_connectionString))
            {
                BookGroup = await connection.QueryAsync<Reservation, User, Book, Book>("sp_GetBookInfoAndBookReservations", (reservation, user, book) =>
                {
                    if (reservation != null)
                        reservation.User = user;
                    book.Reservations = reservation==null? null: new List<Reservation>() { reservation };
                    return book;
                },
                new { bookId },
                commandType: CommandType.StoredProcedure);
            }
            if (!BookGroup.Any()) return null;
            Book book = BookGroup.First();
            book.Reservations = BookGroup.Where(b => b.Reservations != null).Select(b=>b.Reservations.Single()).ToList();
            return book;
        }

        public async Task UpdateBookWithReservations(Book book)
        {
            List<ReservationDTO> reservations = book.Reservations.Select(r => new ReservationDTO(r)).ToList();
            var parameters = new DynamicParameters();
            parameters.AddTable("@reservations", "t_Reservation", reservations);
            parameters.Add("@name", book.Name);
            parameters.Add("@description", book.Description);
            parameters.Add("@inArchive", book.InArchive);
            parameters.Add("@genre", book.Genre);
            parameters.Add("@bookId", book.Id);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync("sp_UpdateBookAndReservations",
                                parameters,
                                commandType: CommandType.StoredProcedure);
            }

        }
    }
}
