using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.Extensions;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.DAL.Interfaces;
using System;
using System.Linq;
using OnlineLibrary.Common.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using FluentValidation;
using OnlineLibrary.Common.EntityProcessing.Pagination;
using LinqKit;
using System.Linq.Expressions;
using OnlineLibrary.Common.EntityProcessing.Filtration;
using OnlineLibrary.Common.EntityProcessing;
using System.Threading.Tasks;

namespace OnlineLibrary.BLL.Services
{
    public class BookServiceEF : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IValidator<Book> _bookValidator;

        public BookServiceEF(IUnitOfWork uow, IValidator<Book> validator)
        {
            _unitOfWork = uow;
            _bookValidator = validator;
        }

        public async Task<PaginatedList<Book>> GetAllBooksAsync(PaginationOptions paginationOptions)
        {
            int count = await _unitOfWork.BookRepository.GetAllBooksCountAsync();
            if (count == 0) return new PaginatedList<Book>();
            int skip = CalculateSkip(count, paginationOptions.PageNumber, paginationOptions.PageSize);
            return new PaginatedList<Book>(count, await _unitOfWork.BookRepository.GetAllBooksAsync(skip, paginationOptions.PageSize));
        }

        private Expression<Func<Book, bool>> Filter(BookFiltration filterBook)
        {
            Expression<Func<Book, bool>> expr = PredicateBuilder.New<Book>(true);

            if (filterBook.AuthorId != null && filterBook.AuthorId > 0)
            {
                expr = expr.And(b => b.Authors.Any(a => a.Id == filterBook.AuthorId));
            }
            if (filterBook.TagId != null && filterBook.TagId > 0)
            {
                expr = expr.And(b => b.Tags.Any(t => t.Id == filterBook.TagId));
            }
            if (filterBook.Name != null && filterBook.Name.Trim() != "")
            {
                expr = expr.And(b => b.Name.Trim().ToUpper().Contains(filterBook.Name.Trim().ToUpper()));
            }
            if (filterBook.Reservation != null && filterBook.Reservation >= 0 && filterBook.Reservation <= 1)
            {
                bool reserve = filterBook.Reservation == 0 ? false : true;
                expr = expr.And(b => b.Reserve == reserve);
            }
            if (filterBook.Archievation != null && filterBook.Archievation >= 0 && filterBook.Archievation <= 1)
            {
                bool archieve = filterBook.Archievation == 0 ? false : true;
                expr = expr.And(b => b.InArchive == archieve);
            }
            return expr;
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

        public async Task<PaginatedList<Book>> FilterBooksAsync(BookProcessing bookProcessing)
        {
            Expression<Func<Book, bool>> expr = Filter(bookProcessing.Filtration);
            int count = await _unitOfWork.BookRepository.GetAllBooksCountAsync(expr);
            ExceptionExtensions.Check<OLNotFound>(count == 0, "There are no books matching this search");
            int skip = CalculateSkip(count, bookProcessing.Pagination.PageNumber, bookProcessing.Pagination.PageSize);
            return new PaginatedList<Book>(count, await _unitOfWork.BookRepository.FilterBooksAsync(expr, skip, bookProcessing.Pagination.PageSize, bookProcessing.Sorting.PropertyToOrder, bookProcessing.Sorting.SortDirection));
        }

        public async Task<Book> GetBookByIdAsync(int? bookId)
        {
            ExceptionExtensions.Check<OLBadRequest>(bookId == null || bookId <= 0, "Id is incorrect");
            Book book = await _unitOfWork.BookRepository.GetBookByIdAsync((int)bookId);
            ExceptionExtensions.Check<OLNotFound>(book == null, "Book not found");
            return book;
        }

        public async Task UpdatePatchAsync(int bookId, JsonPatchDocument<Book> book)
        {
            var originalBook = await _unitOfWork.BookRepository.GetBookByIdAsync(bookId);
            ExceptionExtensions.Check<OLNotFound>(originalBook == null, "Book not found");
            book.ApplyTo(originalBook);
            var results = _bookValidator.Validate(originalBook);
            ExceptionExtensions.Check<OLBadRequest>(!results.IsValid, "The book has been changed incorrectly");
            await _unitOfWork.SaveAsync();
        }

        public async Task<int> CreateBookAsync(Book book)
        {
            ExceptionExtensions.Check<OLBadRequest>(book == null, "A null object came to the method");
            _unitOfWork.BookRepository.InsertBook(book);
            await _unitOfWork.SaveAsync();
            ExceptionExtensions.Check<OLInternalServerError>(book.Id == 0, "The book was not created");
            return book.Id;
        }
    }
}
