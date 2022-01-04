using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using BookAuditNLayerApp.BLL.Interfaces;
using BookAuditNLayerApp.BLL.DTO;
using BookAuditNLayer.GeneralClassLibrary.Entities;

namespace BookAuditNLayerApp.WEB.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService<Book> _bookService;

        private readonly IAuthorService<Author> _authorService;

        public BookController(IBookService<Book> iBook, IAuthorService<Author> iAuthor)
        {
            _bookService = iBook;
            _authorService = iAuthor;
        }

    }
}
