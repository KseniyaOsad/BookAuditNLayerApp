using OnlineLibrary.Common.Entities;
using OnlineLibrary.Common.Enums;
using OnlineLibrary.BLL.Infrastructure;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.BLL.Model;
using OnlineLibrary.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OnlineLibrary.BLL.Services
{
    public class DataExportService : IDataExportService
    {
        private readonly IUnitOfWork unitOfWork;

        public DataExportService(IUnitOfWork uow)
        {
            unitOfWork = uow;
        }

        public void WriteCsv(string path, string filename)
        {
            List<Book> books = unitOfWork.Book.GetAllBooks();
            ExceptionHelper.Check<Exception>(books == null || !books.Any(), "Книг нет");

            using (StreamWriter sw = new StreamWriter(path + filename, false))
            {
                StringBuilder allTextToWrite = new StringBuilder();
                books.
                    Select(
                        b => allTextToWrite.Append(
                            new BookAndAuthorToCSV()
                            {
                                Id = b.Id,
                                Title = b.Name,
                                AuthorName = String.Join(" & ", b.Authors.Select(a => a.Name).ToArray())
                            }.ToString())
                        ).ToList();
                sw.Write(allTextToWrite.ToString());
                sw.Close();
            } 
        }
    }
}
