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

        public async Task<PaginatedList<Book>> FilterBooksAsync(BookProcessing bookProcessing)
        {
            return await _unitOfWork.BookRepository.FilterBooksAsync(bookProcessing);
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
            book.ApplyTo(originalBook);
            ExceptionExtensions.Check<OLBadRequest>(bookId != originalBook.Id, "You are trying to change the ID. it's not allowed.");

            bool updateBook = false, updateAuthors = false, updateTags = false;

            // Take real authors and tags if needed.
            if (book.Operations.Any(x => x.path.Contains("authors")))
            {
                originalBook.Authors = await _unitOfWork.AuthorRepository.GetAuthorsByIdListAsync(originalBook.Authors.Select(x => x.Id).ToList());
                updateAuthors = true;
            }
            if (book.Operations.Any(x => x.path.Contains("tags")))
            {
                originalBook.Tags = await _unitOfWork.TagRepository.GetTagsByIdListAsync(originalBook.Tags.Select(x => x.Id).ToList());
                updateTags = true;
            }
            if (book.Operations.Any(x => !x.path.Contains("tags") && !x.path.Contains("authors")))
            {
                updateBook = true;
            }

            var results = _bookValidator.Validate(originalBook);
            ExceptionExtensions.Check<OLBadRequest>(!results.IsValid, "The book has been changed incorrectly");
            await _unitOfWork.BookRepository.UpdateBookAsync(originalBook, updateBook, updateAuthors, updateTags);
        }

        public async Task<int> CreateBookAsync(Book book)
        {
            ExceptionExtensions.Check<OLBadRequest>(book == null, "A null object came to the method");
            await _unitOfWork.BookRepository.CreateBookAsync(book);
            ExceptionExtensions.Check<OLInternalServerError>(book.Id == 0, "The book was not created");
            return book.Id;
        }
    }
}
