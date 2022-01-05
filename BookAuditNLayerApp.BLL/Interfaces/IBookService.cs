using System;
using System.Collections.Generic;
using System.Text;

namespace BookAuditNLayerApp.BLL.Interfaces
{
    public interface IBookService<T> where T : class
    {
        List<T> GetAllBooks();

        public List<T> FilterBooks(int? authorId, string name, int? inReserve, int? inArchieve);

        T GetBookById(int? bookId);

        void ChangeBookReservation(int? bookId, bool newReservationValue);

        void ChangeBookArchievation(int? bookId, bool newArchievationValue);

        int CreateBook(T book);
    }
}
