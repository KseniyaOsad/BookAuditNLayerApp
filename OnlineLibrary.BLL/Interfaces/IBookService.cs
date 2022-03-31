﻿using Microsoft.AspNetCore.JsonPatch;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.EntityProcessing;
using OnlineLibrary.Common.EntityProcessing.Pagination;
using OnlineLibrary.DAL.DTO;
using System.Threading.Tasks;

namespace OnlineLibrary.BLL.Interfaces
{
    public interface IBookService
    {
        Task<PaginatedList<Book>> FilterSortPaginBooksAsync(BookProcessing bookProcessing);

        Task<Book> GetBookByIdAsync(int bookId);

        Task<int> CreateBookAsync(Book book);

        Task UpdatePatchAsync(int bookId, JsonPatchDocument<BookDTO> book);

    }
}
