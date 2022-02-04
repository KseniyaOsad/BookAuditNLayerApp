using Dapper;
using Microsoft.Data.SqlClient;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.EntityProcessing;
using OnlineLibrary.Common.EntityProcessing.Filtration;
using OnlineLibrary.Common.EntityProcessing.Pagination;
using OnlineLibrary.DAL.Comparer;
using OnlineLibrary.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;

namespace OnlineLibrary.DAL.Repositories.Dapper
{
    public class BookDapperRepository : IBookRepository
    {
        private string _connectionString;

        public BookDapperRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private int CalculateSkip(int count, int pageNumber, int pageSize)
        {
            // If we need to show more data than we have, show the first page.
            if (count < pageSize) return 0;

            // If after skipping data there is some data, show it.
            int skip = (pageNumber - 1) * pageSize;
            if (count - skip >= 1) return skip;

            // Else calculate how much we need to skip for obtaining the last page.
            int remainder = count % pageSize;
            if (remainder == 0) return count - pageSize;
            return count - remainder;
        }

        private string Filter(BookFiltration filterBook)
        {
            List<string> query = new List<string>();

            if (filterBook.AuthorId != null && filterBook.AuthorId > 0)
            {
                query.Add($"AB_w.AuthorsId = {filterBook.AuthorId}");
            }
            if (filterBook.TagId != null && filterBook.TagId > 0)
            {
                query.Add($"BT_w.TagsId = {filterBook.TagId}");
            }
            if (filterBook.Name != null && filterBook.Name.Trim() != "")
            {
                query.Add($"UPPER(B_w.Name) LIKE  '%{filterBook.Name.ToUpper()}%' ");
            }
            if (filterBook.Reservation != null && filterBook.Reservation >= 0 && filterBook.Reservation <= 1)
            {
                query.Add($"B_w.Reserve = {filterBook.Reservation}");
            }
            if (filterBook.Archievation != null && filterBook.Archievation >= 0 && filterBook.Archievation <= 1)
            {
                query.Add($"B_w.InArchive = {filterBook.Archievation}");
            }
            return query.Count() == 0 ? "" : "WHERE " + string.Join(" AND ", query);
        }

        public async Task<PaginatedList<Book>> FilterBooksAsync(BookProcessing bookProcessing)
        {
            IEnumerable<Book> Books;
            int count;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string SortDirection = bookProcessing.Sorting.SortDirection == 0 ? "ASC" : "DESC";
                string whereSql = Filter(bookProcessing.Filtration);
                string fromSelectSql;
                if (!string.IsNullOrEmpty(whereSql))
                {
                    connection.Execute(@"
                    create table #SearchBooks(Id Int, 
                                               Name nvarchar(max), 
                                               Description nvarchar(max), 
                                               Genre INT,
                                                InArchive BIT,
                                                Reserve BIT,
                                                RegistrationDate DATETIME2,
                                                );"
                     );
                    connection.Execute(@$"
                    insert into #SearchBooks
                    SELECT DISTINCT B_w.Id, B_w.Name, B_w.Description, B_w.Genre , B_w.InArchive , B_w.Reserve, B_w.RegistrationDate
                        FROM Book AS B_w
                        LEFT JOIN AuthorBook AS AB_w
                                ON B_w.Id = AB_w.BooksId
                        LEFT JOIN BookTag AS BT_w
                                ON B_w.Id = BT_w.BooksId
                        {whereSql}
                    ");
                    count = connection.ExecuteScalar<int>("SELECT count(*) from #SearchBooks");
                    fromSelectSql = "#SearchBooks";
                }
                else
                {
                    count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Book;");
                    fromSelectSql = "Book";
                }
                int skip = CalculateSkip(count, bookProcessing.Pagination.PageNumber, bookProcessing.Pagination.PageSize);

                string sql = @$"
            SELECT Books.Id, Books.Description, Books.Genre, Books.InArchive, Books.Name, Books.RegistrationDate, Books.Reserve, bookTags.BooksId, bookTags.TagsId, bookTags.Id, bookTags.Name, bookAuthors.AuthorsId, bookAuthors.BooksId, bookAuthors.Id, bookAuthors.Name
            FROM (
                SELECT B.Id, B.Description, B.Genre, B.InArchive, B.Name, B.RegistrationDate, B.Reserve
                FROM 
                   {fromSelectSql}
                AS B
                ORDER BY B.{bookProcessing.Sorting.PropertyToOrder} {SortDirection}
                OFFSET {skip} ROWS FETCH NEXT {bookProcessing.Pagination.PageSize} ROWS ONLY
            ) AS Books
            LEFT JOIN (
                SELECT BT.BooksId, BT.TagsId, T.Id, T.Name
                FROM BookTag AS BT
                INNER JOIN Tag AS T ON BT.TagsId = T.Id
            ) AS bookTags ON Books.Id = bookTags.BooksId
            LEFT JOIN (
                SELECT AB.AuthorsId, AB.BooksId, A.Id, A.Name
                FROM AuthorBook AS AB
                INNER JOIN Author AS A ON AB.AuthorsId = A.Id
            ) AS bookAuthors ON Books.Id = bookAuthors.BooksId
            ORDER BY Books.{bookProcessing.Sorting.PropertyToOrder} {SortDirection}";

                Books = await connection.QueryAsync<Book, Tag, Author, Book>(sql, (book, tag, author) =>
                {
                    book.Authors = author == null ? null : new List<Author>() { author };
                    book.Tags = tag == null ? null : new List<Tag>() { tag };
                    return book;
                });
            }
            
            return new PaginatedList<Book>(count, Books
                .GroupBy(b => b.Id)
                .Select(group =>
                {
                    var book = group.First();
                    book.Authors = group.Where(b => b.Authors != null).Select(b => b.Authors?.Single()).Distinct(new EntityComparer<Author>()).ToList() ?? new List<Author>();
                    book.Tags = group.Where(b => b.Tags != null).Select(b => b.Tags.Single()).Distinct(new EntityComparer<Tag>()).ToList() ?? new List<Tag>();
                    return book;
                })
                .ToList());
        }

        public Task<List<Book>> FilterBooksAsync(Expression<Func<Book, bool>> expr, int skip, int pageSize, string propertyToOrder, ListSortDirection SortDirection)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Book>> GetAllBooksAsync(int skip, int pageSize)
        {
            IEnumerable<Book> Books;
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @$"Select B.Id, B.Name, T.Id, T.Name, A.Id, A.Name
                    FROM Book AS B
                    LEFT JOIN AuthorBook AS AB
                            ON B.Id = AB.BooksId
                            LEFT JOIN Author AS A
                                ON AB.AuthorsId = A.Id
                    LEFT JOIN BookTag AS BT
                            ON B.Id = BT.BooksId
                            LEFT JOIN Tag AS T
                                ON BT.TagsId = T.Id 
                    OFFSET     {skip} ROWS      
                    FETCH NEXT {pageSize} ROWS ONLY; ";

                Books = await connection.QueryAsync<Book, Tag, Author, Book>(sql, (book, tag, author) => {
                    book.Authors = author == null ? null : new List<Author>() { author };
                    book.Tags= tag == null ? null : new List<Tag>() { tag };
                    return book;
                });
            }
            return Books
                .GroupBy(b => b.Id)
                .Select(group=> {
                    var book = group.First();
                    book.Authors = group.Where(b=>b.Authors != null).Select(b => b.Authors?.Single()).Distinct(new EntityComparer<Author>()).ToList() ?? new List<Author>();
                    book.Tags = group.Where(b => b.Tags != null).Select(b => b.Tags.Single()).Distinct(new EntityComparer<Tag>()).ToList() ?? new List<Tag>();
                    return book;
                })
                .ToList();
        }

        public async Task<int> GetAllBooksCountAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Book;");
            }
        }

        public async Task<int> GetAllBooksCountAsync(Expression<Func<Book, bool>> expr)
        {
            throw new NotImplementedException();
        }

        public async Task<Book> GetBookByIdAsync(int bookId)
        {
            string sql = @$"Select B.Id, B.Name, B.Description, B.Reserve, B.InArchive, B.Genre, B.RegistrationDate ,T.Id, T.Name, A.Id, A.Name
                    FROM Book AS B
                    LEFT JOIN AuthorBook AS AB
                            ON B.Id = AB.BooksId
                            LEFT JOIN Author AS A
                                ON AB.AuthorsId = A.Id
                    LEFT JOIN BookTag AS BT
                            ON B.Id = BT.BooksId
                            LEFT JOIN Tag AS T
                                ON BT.TagsId = T.Id 
                    WHERE B.Id = {bookId}";

            IEnumerable<Book> BookGroup;
            using (var connection = new SqlConnection(_connectionString))
            {
                BookGroup = await connection.QueryAsync<Book, Tag, Author, Book>(sql, (book, tag, author) =>
                {
                    book.Authors = author == null ? null : new List<Author>() { author };
                    book.Tags = tag == null ? null : new List<Tag>() { tag };
                    return book;
                });
            }
            Book book = BookGroup.First();
            book.Authors = BookGroup.Where(b => b.Authors != null).Select(b => b.Authors?.Single()).Distinct(new EntityComparer<Author>()).ToList() ?? new List<Author>();
            book.Tags = BookGroup.Where(b => b.Tags != null).Select(b => b.Tags.Single()).Distinct(new EntityComparer<Tag>()).ToList() ?? new List<Tag>();
            return book;
        }

        public void InsertBook(Book book)
        {
            using (var scope = new TransactionScope())
                using (var connection = new SqlConnection(_connectionString))
                {
                    string createBookSql = "INSERT INTO Book (Name, Description, Reserve, InArchive, Genre, RegistrationDate) Values (@Name, @Description, @Reserve, @InArchive, @Genre, @RegistrationDate); SELECT SCOPE_IDENTITY();";
                    book.Id = connection.ExecuteScalar<int>(createBookSql, book);

                    string createAuthorBookSql = $"INSERT INTO AuthorBook (AuthorsId, BooksId) Values (@AuthorsId,{book.Id});";
                    var affectedRows = connection.Execute(createAuthorBookSql, book.Authors.Select(a => new { AuthorsId = a.Id }));

                    string createBookTagSql = $"INSERT INTO BookTag (TagsId, BooksId) Values (@TagsId,{book.Id});";
                    affectedRows = connection.Execute(createBookTagSql, book.Tags.Select(t => new { TagsId = t.Id }));

                    scope.Complete();
                }
        }
    }
}
