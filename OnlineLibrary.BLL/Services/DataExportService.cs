using OnlineLibrary.Common.Entities;
using OnlineLibrary.Common.Helpers;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.BLL.Model;
using OnlineLibrary.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OnlineLibrary.Common.Exceptions.Enum;
using OnlineLibrary.Common.Exceptions;

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
            List<Book> books = unitOfWork.BookRepository.GetAllBooks(0, 50);
            ExceptionHelper.Check<OLNotFound>(books == null || !books.Any(), "Books don't exist");
            ExceptionHelper.Check<OLInternalServerError>(path == null || filename == null || path.Trim() == "" || filename.Trim() == "", "File path is empty");
            try
            {
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
            catch (Exception ex)
            {
                throw new OLInternalServerError("Failed to write file: " + ex.ToString());
            }
        }
    }
}
