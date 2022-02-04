using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.Extensions;
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
using System.Threading.Tasks;

namespace OnlineLibrary.BLL.Services
{
    public class DataExportService : IDataExportService
    {
        private readonly IUnitOfWork unitOfWork;

        public DataExportService(IUnitOfWork uow)
        {
            unitOfWork = uow;
        }

        public async Task WriteCsvAsync(string path, string filename)
        {
            int count = await unitOfWork.BookRepository.GetAllBooksCountAsync();
            ExceptionExtensions.Check<OLNotFound>(count == 0, "Books don't exist");
            List<Book> books = await unitOfWork.BookRepository.GetAllBooksAsync(0, count);
            ExceptionExtensions.Check<OLInternalServerError>(path == null || filename == null || path.Trim() == "" || filename.Trim() == "", "File path is empty");
            try
            {
                using (StreamWriter sw = new StreamWriter(path + filename, false))
                {
                    StringBuilder allTextToWrite = new StringBuilder();
                    books.
                        Select(
                            b => allTextToWrite.Append(
                                new BookInfoToCSV()
                                {
                                    Id = b.Id,
                                    Title = b.Name,
                                    AuthorNames = String.Join(" & ", b.Authors.Select(a => a.Name).ToArray()),
                                    TagNames = String.Join(" & ", b.Tags.Select(t => t.Name).ToArray())
                                }.ToString())
                            ).ToList();
                    await sw.WriteAsync(allTextToWrite.ToString());
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
