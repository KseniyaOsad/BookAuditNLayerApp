using BookAuditNLayer.GeneralClassLibrary.Entities;
using BookAuditNLayer.GeneralClassLibrary.Enums;
using BookAuditNLayerApp.BLL.Infrastructure;
using BookAuditNLayerApp.BLL.Interfaces;
using BookAuditNLayerApp.BLL.Model;
using BookAuditNLayerApp.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BookAuditNLayerApp.BLL.Services
{
    public class DataExportService : IDataExportService
    {
        IUnitOfWork Database { get; set; }

        public DataExportService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public void WriteCsv(string path, string filename)
        {
            List<Book> books = Database.Book.GetAllBooks();
            if (books.Any())
            {
                StreamWriter sw = new StreamWriter(path + filename, false);
                StringBuilder allTextToWrite = new StringBuilder();
                books.Select(
                        b => allTextToWrite.Append(
                            new BookAndAuthorToCSV()
                            {
                                Id = b.Id,
                                Title = b.Name,
                                AuthorName = b.Author.Name
                            }.ToString())
                        ).ToList();
                sw.Write(allTextToWrite.ToString());
                sw.Close();
            }
            else
            {
                throw new ValidationException("Книг нет", ErrorList.ListIsEmpty);
            }
        }
    }
}
