using OnlineLibrary.Common.Entities;
using OnlineLibrary.Common.Helpers;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.Common.Exceptions.Enum;
using Microsoft.AspNetCore.JsonPatch;
using OnlineLibrary.Common.Validators;
using FluentValidation;
using OnlineLibrary.Common.Pagination;
using LinqKit;
using System.Linq.Expressions;

namespace OnlineLibrary.BLL.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IValidator<Book> bookValidator;

        public BookService(IUnitOfWork uow, IValidator<Book> validator)
        {
            unitOfWork = uow;
            bookValidator = validator;
        }

        public List<Book> GetAllBooks()
        {
            List<Book> books = unitOfWork.BookRepository.GetAllBooks();
            return books;
        }

        public PaginatedList<Book> GetAllBooks(PaginationOptions paginationOptions)
        {
            int count = unitOfWork.BookRepository.GetAllBooksCount();
            if (count == 0) return new PaginatedList<Book>();
            int skip = (paginationOptions.PageNumber - 1) * paginationOptions.PageSize;

            if (count - skip >= 1)
            {
                return new PaginatedList<Book>(count, unitOfWork.BookRepository.GetAllBooks(skip, paginationOptions.PageSize));
            }
            else
            {
                int page = count == paginationOptions.PageSize ? 1 : (count / paginationOptions.PageSize) + 1;
                skip = (page - 1) * paginationOptions.PageSize;
                return new PaginatedList<Book>(count, unitOfWork.BookRepository.GetAllBooks(skip, paginationOptions.PageSize));
            }
        }

        private Expression<Func<Book, bool>> Filter(int? authorId, string name, int? inReserve, int? inArchieve)
        {
            Expression<Func<Book, bool>> expr = PredicateBuilder.New<Book>(true);
            if (authorId != null && authorId > 0)
            {
                expr = expr.And(b => b.Authors.Any(a => a.Id == authorId));
            }
            if (name != null && name.Trim() != "")
            {
                expr = expr.And(b => b.Name.Trim().ToUpper().Contains(name.Trim().ToUpper()));
            }
            if (inReserve != null && inReserve >= 0 && inReserve <= 1)
            {
                bool reserve = inReserve == 0 ? false : true;
                expr = expr.And(b => b.Reserve == reserve);
            }
            if (inArchieve != null && inArchieve >= 0 && inArchieve <= 1)
            {
                bool archieve = inArchieve == 0 ? false : true;
                expr = expr.And(b => b.InArchive == archieve);
            }
            return expr;
        }

        public PaginatedList<Book> FilterBooks(int? authorId, string name, int? inReserve, int? inArchieve, PaginationOptions paginationOptions)
        {
            Expression<Func<Book, bool>> expr = Filter(authorId, name, inReserve, inArchieve);
            int count = unitOfWork.BookRepository.GetAllBooksCount(expr);
            ExceptionHelper.Check<OLNotFound>(count == 0, "There are no books matching this search");
            int skip = (paginationOptions.PageNumber - 1) * paginationOptions.PageSize;

            if (count - skip >= 1)
            {
                return new PaginatedList<Book>(count, unitOfWork.BookRepository.FilterBooks(expr, skip, paginationOptions.PageSize));
            }
            else
            {
                int page = count == paginationOptions.PageSize ? 1 : (count / paginationOptions.PageSize) + 1;
                skip = (page - 1) * paginationOptions.PageSize;
                return new PaginatedList<Book>(count, unitOfWork.BookRepository.FilterBooks(expr, skip, paginationOptions.PageSize));
            }
        }

        public List<Book> FilterBooks(int? authorId, string name, int? inReserve, int? inArchieve)
        {
            List<Book> books = unitOfWork.BookRepository.FilterBooks(Filter(authorId, name, inReserve, inArchieve));
            ExceptionHelper.Check<OLNotFound>(books == null || !books.Any(), "There are no books matching this search");
            return books;
        }

        public Book GetBookById(int? bookId)
        {
            ExceptionHelper.Check<OLBadRequest>(bookId == null || bookId <= 0, "Id is incorrect");
            Book book = unitOfWork.BookRepository.GetBookById((int)bookId);
            ExceptionHelper.Check<OLNotFound>(book == null, "Book not found");
            return book;
        }

        public void ChangeBookReservation(int? bookId, bool newReservationValue)
        {
            ExceptionHelper.Check<OLBadRequest>(bookId == null || bookId <= 0, "Id is incorrect");
            ExceptionHelper.Check<OLNotFound>(!unitOfWork.BookRepository.IsBookIdExists((int)bookId), "Book not found");
            unitOfWork.BookRepository.ChangeBookReservation((int)bookId, newReservationValue);
            unitOfWork.Save();
        }

        public void ChangeBookArchievation(int? bookId, bool newArchievationValue)
        {
            ExceptionHelper.Check<OLBadRequest>(bookId == null || bookId <= 0, "Id is incorrect");
            ExceptionHelper.Check<OLNotFound>(!unitOfWork.BookRepository.IsBookIdExists((int)bookId), "Book not found");
            unitOfWork.BookRepository.ChangeBookArchievation((int)bookId, newArchievationValue);
            unitOfWork.Save();
        }
        public void UpdatePatch(int bookId, JsonPatchDocument<Book> book)
        {
            var originalBook = unitOfWork.BookRepository.GetBookById(bookId);
            ExceptionHelper.Check<OLNotFound>(originalBook == null, "Book not found");
            book.ApplyTo(originalBook);
            var results = bookValidator.Validate(originalBook);
            ExceptionHelper.Check<OLBadRequest>(!results.IsValid, "The book has been changed incorrectly");
            unitOfWork.Save();
        }

        public int CreateBook(Book book)
        {
            ExceptionHelper.Check<OLBadRequest>(book == null, "A null object came to the method");
            unitOfWork.BookRepository.InsertBook(book);
            unitOfWork.Save();
            ExceptionHelper.Check<OLInternalServerError>(book.Id == 0, "The book was not created");
            return book.Id;
        }
    }
}
