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

        public BookService(IUnitOfWork uow, IValidator<Book> validator )
        {
            unitOfWork = uow;
            bookValidator = validator;
        }

        public List<Book> GetAllBooks()
        {
            List<Book> books = unitOfWork.BookRepository.GetAllBooks();
            return books;
        }

        public PageOfEntities<Book> GetAllBooks(PaginationOptions paginationOptions)
        {
            int count = unitOfWork.BookRepository.GetAllBooksCount();
            if (count == 0) return new PageOfEntities<Book>();
            int skip = (paginationOptions.PageNumber - 1) * paginationOptions.PageSize;
            PageOfEntities<Book> page = new PageOfEntities<Book>();
            page.EntitiesCount = count;

            if (count - skip >= 1)
            {
                page.Entities = unitOfWork.BookRepository.GetAllBooks(skip, paginationOptions.PageSize);
                page.Page = paginationOptions.PageNumber;
            }
            else
            {
                page.Page = (count / paginationOptions.PageSize) + 1;
                skip = (page.Page - 1) * paginationOptions.PageSize;
                page.Entities = unitOfWork.BookRepository.GetAllBooks(skip, paginationOptions.PageSize);
            }
            page.ElementsOnPage = page.Entities.Count();
            return page;
        }

        public List<Book> FilterBooks(int? authorId, string name, int? inReserve, int? inArchieve)
        {
            Expression<Func<Book, bool>> expr = PredicateBuilder.New<Book>(true);
            if (authorId != null && authorId > 0)
            {
                expr = expr.And(b=>b.Authors.Contains(unitOfWork.AuthorRepository.GetAuthorsByIdList(new List<int>(){ (int)authorId}).First())); // ????????????
            }
            if (name != null && name.Trim() != "")
            {
                expr = expr.And(b => b.Name.Trim().ToLower().Contains(name.Trim().ToLower()));
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
            List<Book> books = unitOfWork.BookRepository.FilterBooks(expr);
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
