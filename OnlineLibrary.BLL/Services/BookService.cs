using OnlineLibrary.Common.Entities;
using OnlineLibrary.Common.Helpers;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using OnlineLibrary.Common.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using FluentValidation;
using OnlineLibrary.Common.Pagination;
using LinqKit;
using System.Linq.Expressions;
using System.ComponentModel;

namespace OnlineLibrary.BLL.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IValidator<Book> _bookValidator;

        public BookService(IUnitOfWork uow, IValidator<Book> validator)
        {
            _unitOfWork = uow;
            _bookValidator = validator;
        }

        public PaginatedList<Book> GetAllBooks(PaginationOptions paginationOptions)
        {
            int count = _unitOfWork.BookRepository.GetAllBooksCount();
            if (count == 0) return new PaginatedList<Book>();
            int skip = (paginationOptions.PageNumber - 1) * paginationOptions.PageSize;

            if (count - skip >= 1) // Check if there are items on this page.
            {
                return new PaginatedList<Book>(count, _unitOfWork.BookRepository.GetAllBooks(skip, paginationOptions.PageSize));
            }
            else // If no elements are found on the specified page return the latest page.
            {
                int page = count == paginationOptions.PageSize ? 1 : (count / paginationOptions.PageSize) + 1;
                skip = (page - 1) * paginationOptions.PageSize;
                return new PaginatedList<Book>(count, _unitOfWork.BookRepository.GetAllBooks(skip, paginationOptions.PageSize));
            }
        }

        private Expression<Func<Book, bool>> Filter(FilterBook filterBook)
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

        public PaginatedList<Book> FilterBooks(FilterBook filterBook)
        {
            Expression<Func<Book, bool>> expr = Filter(filterBook);
            int count = _unitOfWork.BookRepository.GetAllBooksCount(expr);
            ExceptionHelper.Check<OLNotFound>(count == 0, "There are no books matching this search");
            int skip = (filterBook.Pagination.PageNumber - 1) * filterBook.Pagination.PageSize;

            // If SortDirection or PropertyToOrder are not setted => set standard values.
            filterBook.SortDirection = 
                (filterBook.SortDirection != null && Enum.IsDefined(typeof(ListSortDirection), filterBook.SortDirection)) 
                ? filterBook.SortDirection : ListSortDirection.Ascending;
            filterBook.PropertyToOrder = 
                (filterBook.PropertyToOrder != null && filterBook.PropertyToOrder.Trim() != "") 
                ? filterBook.PropertyToOrder.Trim() : "Id" ;

            
            if (count - skip >= 1) // Check if there are items on this page.
            {
                return new PaginatedList<Book>(count, _unitOfWork.BookRepository.FilterBooks(expr, skip, filterBook.Pagination.PageSize, filterBook.PropertyToOrder, (ListSortDirection)filterBook.SortDirection));
            }
            else // If no elements are found on the specified page return the latest page.
            {
                int page = count == filterBook.Pagination.PageSize ? 1 : (count / filterBook.Pagination.PageSize) + 1;
                skip = (page - 1) * filterBook.Pagination.PageSize;
                return new PaginatedList<Book>(count, _unitOfWork.BookRepository.FilterBooks(expr, skip, filterBook.Pagination.PageSize, filterBook.PropertyToOrder, (ListSortDirection)filterBook.SortDirection));
            }
        }

        public Book GetBookById(int? bookId)
        {
            ExceptionHelper.Check<OLBadRequest>(bookId == null || bookId <= 0, "Id is incorrect");
            Book book = _unitOfWork.BookRepository.GetBookById((int)bookId);
            ExceptionHelper.Check<OLNotFound>(book == null, "Book not found");
            return book;
        }

        public void ChangeBookReservation(int? bookId, bool newReservationValue)
        {
            ExceptionHelper.Check<OLBadRequest>(bookId == null || bookId <= 0, "Id is incorrect");
            ExceptionHelper.Check<OLNotFound>(!_unitOfWork.BookRepository.IsBookIdExists((int)bookId), "Book not found");
            _unitOfWork.BookRepository.ChangeBookReservation((int)bookId, newReservationValue);
            _unitOfWork.Save();
        }

        public void ChangeBookArchievation(int? bookId, bool newArchievationValue)
        {
            ExceptionHelper.Check<OLBadRequest>(bookId == null || bookId <= 0, "Id is incorrect");
            ExceptionHelper.Check<OLNotFound>(!_unitOfWork.BookRepository.IsBookIdExists((int)bookId), "Book not found");
            _unitOfWork.BookRepository.ChangeBookArchievation((int)bookId, newArchievationValue);
            _unitOfWork.Save();
        }
        public void UpdatePatch(int bookId, JsonPatchDocument<Book> book)
        {
            var originalBook = _unitOfWork.BookRepository.GetBookById(bookId);
            ExceptionHelper.Check<OLNotFound>(originalBook == null, "Book not found");
            book.ApplyTo(originalBook);
            var results = _bookValidator.Validate(originalBook);
            ExceptionHelper.Check<OLBadRequest>(!results.IsValid, "The book has been changed incorrectly");
            _unitOfWork.Save();
        }

        public int CreateBook(Book book)
        {
            ExceptionHelper.Check<OLBadRequest>(book == null, "A null object came to the method");
            _unitOfWork.BookRepository.InsertBook(book);
            _unitOfWork.Save();
            ExceptionHelper.Check<OLInternalServerError>(book.Id == 0, "The book was not created");
            return book.Id;
        }
    }
}
