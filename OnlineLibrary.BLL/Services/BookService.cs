using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.Extensions;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.DAL.Interfaces;
using OnlineLibrary.Common.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using FluentValidation;
using OnlineLibrary.Common.EntityProcessing.Pagination;
using OnlineLibrary.Common.EntityProcessing;
using System.Threading.Tasks;
using System.Linq;
using FluentValidation.Results;

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

        public Task<PaginatedList<Book>> FilterBooksAsync(BookProcessing bookProcessing)
        {
            return _unitOfWork.BookRepository.FilterBooksAsync(bookProcessing);
        }

        public async Task<Book> GetBookByIdAsync(int bookId)
        {
            Book book = await _unitOfWork.BookRepository.GetBookByIdAsync(bookId);
            ExceptionExtensions.Check<OLNotFound>(book == null, "Book not found");
            return book;
        }

        public async Task UpdatePatchAsync(int bookId, JsonPatchDocument<Book> book)
        {
            var originalBook = await _unitOfWork.BookRepository.GetBookByIdAsync(bookId);
            ExceptionExtensions.Check<OLNotFound>(originalBook == null, "Book not found");
            book.ApplyTo(originalBook);
            ExceptionExtensions.Check<OLBadRequest>(bookId != originalBook.Id, "You are trying to change the ID. it's not allowed.");

            ValidationResult results = _bookValidator.Validate(originalBook);
            ExceptionExtensions.Check<OLBadRequest>(!results.IsValid, "The book has been changed incorrectly");
            
            bool updateBook = false, updateAuthors = false, updateTags = false;

            // Take real authors and tags if needed.
            if (book.Operations.Any(x => x.path.Contains("authors")))
            {
                originalBook.Authors = await _unitOfWork.AuthorRepository.GetAuthorsByIdListAsync(originalBook.Authors.Select(x => x.Id).ToList());
                ExceptionExtensions.Check<OLBadRequest>(!originalBook.Authors.Any(), "You are trying to change the Authors. But this authors don't exists.");
                updateAuthors = true;
            }
            if (book.Operations.Any(x => x.path.Contains("tags")))
            {
                originalBook.Tags = await _unitOfWork.TagRepository.GetTagsByIdListAsync(originalBook.Tags.Select(x => x.Id).ToList());
                ExceptionExtensions.Check<OLBadRequest>(!originalBook.Tags.Any(), "You are trying to change the Tags. But this tags don't exists.");
                updateTags = true;
            }
            if (book.Operations.Any(x => !x.path.Contains("tags") && !x.path.Contains("authors")))
            {
                if (book.Operations.Any(x => x.path.Contains("inArchive")))
                {
                    ExceptionExtensions.Check<OLBadRequest>( await _unitOfWork.ReservationRepository.IsBookInReserve(bookId), "You can't archivate this book. It is in reserve.");
                }
                updateBook = true;
            }

            await _unitOfWork.BookRepository.UpdateBookAsync(originalBook, updateBook, updateAuthors, updateTags);
        }

        public async Task<int> CreateBookAsync(Book book)
        {
            await _unitOfWork.BookRepository.CreateBookAsync(book);
            ExceptionExtensions.Check<OLInternalServerError>(book.Id == 0, "The book was not created");
            return book.Id;
        }
    }
}
