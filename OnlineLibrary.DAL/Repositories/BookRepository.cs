using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using OnlineLibrary.Common.Connection;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.EntityProcessing;
using OnlineLibrary.Common.EntityProcessing.Filtration;
using OnlineLibrary.Common.EntityProcessing.Pagination;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.Common.Extensions;
using OnlineLibrary.DAL.Comparer;
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
                    await connection.ExecuteAsync(@"
                    create table #SearchBooks(Id Int, 
                                               Name nvarchar(max), 
                                               Description nvarchar(max), 
                                               Genre INT,
                                                InArchive BIT,
                                                RegistrationDate DATETIME2,
                                                );"
                     );
                    await connection.ExecuteAsync(@$"
                    insert into #SearchBooks
                    SELECT DISTINCT B_w.Id, B_w.Name, B_w.Description, B_w.Genre , B_w.InArchive , B_w.RegistrationDate
                        FROM Books AS B_w
                        LEFT JOIN AuthorBook AS AB_w
                                ON B_w.Id = AB_w.BooksId
                        LEFT JOIN BookTag AS BT_w
                                ON B_w.Id = BT_w.BooksId
                        {whereSql}
                    ");
                    count = await connection.ExecuteScalarAsync<int>("SELECT count(*) from #SearchBooks");
                    fromSelectSql = "#SearchBooks";
                }
                else
                {
                    count = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Books;");
                    fromSelectSql = "Books";
                }
                ExceptionExtensions.Check<OLNotFound>(count == 0, "Books not found");

                int skip = CalculateSkip(count, bookProcessing.Pagination.PageNumber, bookProcessing.Pagination.PageSize);

                string sql = @$"
            SELECT Books.Id, Books.Description, Books.Genre, Books.InArchive, Books.Name, Books.RegistrationDate, bookTags.BooksId, bookTags.TagsId, bookTags.Id, bookTags.Name, bookAuthors.AuthorsId, bookAuthors.BooksId, bookAuthors.Id, bookAuthors.Name
            FROM (
                SELECT B.Id, B.Description, B.Genre, B.InArchive, B.Name, B.RegistrationDate
                FROM 
                   {fromSelectSql}
                AS B
                ORDER BY B.{bookProcessing.Sorting.PropertyToOrder} {SortDirection}
                OFFSET {skip} ROWS FETCH NEXT {bookProcessing.Pagination.PageSize} ROWS ONLY
            ) AS Books
            LEFT JOIN (
                SELECT BT.BooksId, BT.TagsId, T.Id, T.Name
                FROM BookTag AS BT
                INNER JOIN Tags AS T ON BT.TagsId = T.Id
            ) AS bookTags ON Books.Id = bookTags.BooksId
            LEFT JOIN (
                SELECT AB.AuthorsId, AB.BooksId, A.Id, A.Name
                FROM AuthorBook AS AB
                INNER JOIN Authors AS A ON AB.AuthorsId = A.Id
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

        public async Task<List<Book>> GetAllBooksForCsvAsync()
        {
            IEnumerable<Book> Books;
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @$"Select B.Id, B.Name, T.Id, T.Name, A.Id, A.Name
                    FROM Books AS B
                    LEFT JOIN AuthorBook AS AB
                            ON B.Id = AB.BooksId
                            LEFT JOIN Authors AS A
                                ON AB.AuthorsId = A.Id
                    LEFT JOIN BookTag AS BT
                            ON B.Id = BT.BooksId
                            LEFT JOIN Tags AS T
                                ON BT.TagsId = T.Id; ";

                Books = await connection.QueryAsync<Book, Tag, Author, Book>(sql, (book, tag, author) =>
                {
                    book.Authors = author == null ? null : new List<Author>() { author };
                    book.Tags = tag == null ? null : new List<Tag>() { tag };
                    return book;
                });
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
            string sql = @$"Select B.Id, B.Name, B.Description, B.InArchive, B.Genre, B.RegistrationDate ,T.Id, T.Name, A.Id, A.Name
                    FROM Books AS B
                    LEFT JOIN AuthorBook AS AB
                            ON B.Id = AB.BooksId
                            LEFT JOIN Authors AS A
                                ON AB.AuthorsId = A.Id
                    LEFT JOIN BookTag AS BT
                            ON B.Id = BT.BooksId
                            LEFT JOIN Tags AS T
                                ON BT.TagsId = T.Id 
                    WHERE B.Id = @BookId";

            IEnumerable<Book> BookGroup;
            using (var connection = new SqlConnection(_connectionString))
            {
                BookGroup = await connection.QueryAsync<Book, Tag, Author, Book>(sql, (book, tag, author) =>
                {
                    book.Authors = author == null ? null : new List<Author>() { author };
                    book.Tags = tag == null ? null : new List<Tag>() { tag };
                    return book;
                }, new { BookId = bookId });
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
                    string createBookSql = "INSERT INTO Books (Name, Description, InArchive, Genre, RegistrationDate) Values (@Name, @Description, @InArchive, @Genre, @RegistrationDate); SELECT SCOPE_IDENTITY();";
                    book.Id = await connection.ExecuteScalarAsync<int>(createBookSql, book);

                    string createAuthorBookSql = $"INSERT INTO AuthorBook (AuthorsId, BooksId) Values (@AuthorsId, @BooksId);";
                    await connection.ExecuteAsync(createAuthorBookSql, book.Authors.Select(a => new { AuthorsId = a.Id, BooksId = book.Id }));

                    string createBookTagSql = $"INSERT INTO BookTag (TagsId, BooksId) Values (@TagsId,@BooksId);";
                    await connection.ExecuteAsync(createBookTagSql, book.Tags.Select(t => new { TagsId = t.Id, BooksId = book.Id }));
                    
                    scope.Complete();
                }
            }
        }

        private async Task UpdateAuthorBook(SqlConnection connection, Book book)
        {
            // Find all existing authors in AuthorBook.
            string getAuthorBookEntities = "Select AuthorsId FROM AuthorBook WHERE BooksId = @BooksId";
            List<int> oldAuthors = (await connection.QueryAsync<int>(getAuthorBookEntities, new { BooksId = book.Id })).ToList();

            // Create new row if needed.
            string createAuthorBookSql = $"INSERT INTO AuthorBook (AuthorsId, BooksId) Values (@AuthorsId, @BooksId);";
            foreach (var newAuthor in book.Authors)
            {
                if (oldAuthors.Contains(newAuthor.Id))
                {
                    oldAuthors.Remove(newAuthor.Id);
                }
                else
                {
                    await connection.ExecuteAsync(createAuthorBookSql, new { AuthorsId = newAuthor.Id, BooksId = book.Id });
                }
            }

            // Delete old rows if needed.
            if (oldAuthors.Any())
            {
                string delteOldAuthorBookSql = $"DELETE FROM AuthorBook WHERE AuthorsId = @AuthorsId;";
                await connection.ExecuteAsync(delteOldAuthorBookSql, oldAuthors.Select(x => new { AuthorsId = x }));
            }
        }

        private async Task UpdateBookTag(SqlConnection connection, Book book)
        {
            // Find all existing tags in BookTag.
            string getBookTagEntities = "Select TagsId FROM BookTag WHERE BooksId = @BooksId";
            List<int> oldTags = (await connection.QueryAsync<int>(getBookTagEntities, new { BooksId = book.Id })).ToList();

            // Create new row if needed.
            string createBookTagSql = $"INSERT INTO BookTag (TagsId, BooksId) Values (@TagsId, @BooksId);";
            foreach (var newTag in book.Tags)
            {
                if (oldTags.Contains(newTag.Id))
                {
                    oldTags.Remove(newTag.Id);
                }
                else
                {
                    await connection.ExecuteAsync(createBookTagSql, new { TagsId = newTag.Id, BooksId = book.Id });
                }
            }

            // Delete old rows if needed.
            if (oldTags.Any())
            {
                string delteOldTagBookSql = $"DELETE FROM BookTag WHERE TagsId = @TagsId;";
                await connection.ExecuteAsync(delteOldTagBookSql, oldTags.Select(x => new { TagsId = x }));
            }
        }

        public async Task UpdateBookAsync(Book book, bool updateBook, bool updateAuthors, bool updateTags)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Update book only if we make some changes with it.
                if (updateBook)
                {
                    string updateBookSql = "UPDATE Books SET Name = @Name, Description=@Description, InArchive=@InArchive, Genre=@Genre WHERE Id = @Id ";
                    await connection.ExecuteAsync(updateBookSql, book);
                }

                if (updateAuthors) await UpdateAuthorBook(connection, book);
                if (updateTags) await UpdateBookTag(connection, book);
            }
        }

        public async Task<bool> IsBookExistAsync(int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return (await connection.QueryAsync<object>(
                    "SELECT 1 FROM Books WHERE Id = @Id;", new { Id = userId }))
                    .Any();
            }
        }
    }
}
