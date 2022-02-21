using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using OnlineLibrary.Common.Connection;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.Common.Extensions;
using OnlineLibrary.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLibrary.DAL.Repositories.Dapper
{
    class ReservationRepository : IReservationRepository
    {
        private readonly string _connectionString;

        public ReservationRepository(IOptions<DBConnection> connOptions)
        {
            _connectionString = connOptions.Value.BookContext;
        }

        public async Task CloseReservationAsync(Reservation reservation)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string updateRow = @"
                UPDATE Reservations SET ReturnDate = @ReturnDate WHERE Id = @ResId; 
                ";
                await connection.ExecuteAsync(updateRow, new { ReturnDate = DateTime.Now, ResId = reservation.Id });
            }
        }

        public async Task<Reservation> GetBookReservationLastRow(int bookId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string getRowSql = @"
                    Select TOP 1 Res.Id, Res.UserId, Res.ReturnDate FROM Reservations AS Res
                    WHERE Res.BookId = @BookId 
                    ORDER BY Res.Id DESC;
                ";
                return await connection.QueryFirstOrDefaultAsync<Reservation>(getRowSql, new { BookId = bookId });
            }
        }

        public async Task CreateReservationAsync(Reservation reservation)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // check if book is in reserve.
                bool isInHistory = (await connection.QueryAsync<object>(
                    "SELECT 1 FROM Reservations WHERE BookId = @BookId;", new { BookId = reservation.Book.Id }))
                    .Any();

                if (isInHistory)
                {
                    string sqlGetLastReturnDate = @"
                    Select TOP 1 ReturnDate FROM Reservations 
                    WHERE BookId = @BookId 
                    ORDER BY Id DESC;";
                    DateTime returnDate = await connection.ExecuteScalarAsync<DateTime>(sqlGetLastReturnDate, new { BookId = reservation.Book.Id });
                    ExceptionExtensions.Check<OLBadRequest>(returnDate == default, $"Book is in reserve.");
                }

                string sql = "INSERT INTO Reservations (UserId, BookId, ReservationDate) Values (@UserId, @BookId, @ReservationDate);  SELECT SCOPE_IDENTITY();";
                reservation.Id = await connection.ExecuteScalarAsync<int>(sql, new { UserId = reservation.User.Id, BookId = reservation.Book.Id, ReservationDate = reservation.ReservationDate });
            }
        }

        public async Task<List<Reservation>> GetAllReservationsAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Res.Id, Res.UserId, Res.BookId, Res.ReturnDate, Res.ReservationDate, U.Id, U.Name, U.Email, U.RegistrationDate, U.DateOfBirth, B.Id, B.Name, B.Description, B.Genre , B.InArchive , B.RegistrationDate 
                FROM Reservations AS Res
                LEFT JOIN Users AS U   ON U.Id = Res.UserId
                LEFT JOIN Books AS B   ON B.Id = Res.BookId
                ORDER BY Res.Id DESC;";
                return (await connection.QueryAsync<Reservation, User, Book, Reservation>(sql, (reservation, user, book) =>
                {
                    reservation.Book = book;
                    reservation.User = user;
                    return reservation;
                })).ToList();
            }
        }

        public async Task<List<Reservation>> GetBookReservationHistoryAsync(int bookId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Res.Id, Res.UserId, Res.BookId, Res.ReturnDate, Res.ReservationDate, U.Id, U.Name, U.Email, U.RegistrationDate, U.DateOfBirth, B.Id, B.Name, B.Description, B.Genre , B.InArchive , B.RegistrationDate 
                FROM Reservations AS Res
                LEFT JOIN Users AS U   ON U.Id = Res.UserId
                LEFT JOIN Books AS B   ON B.Id = Res.BookId
                WHERE Res.BookId = @bookId
                ORDER BY Res.Id DESC;";
                return (await connection.QueryAsync<Reservation, User, Book, Reservation>(sql, (reservation, user, book) =>
                {
                    reservation.Book = book;
                    reservation.User = user;
                    return reservation;
                }, new { bookId = bookId })).ToList();
            }
        }

        public async Task<List<Reservation>> GetUserReservationHistoryAsync(int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Res.Id, Res.UserId, Res.BookId, Res.ReturnDate, Res.ReservationDate, U.Id, U.Name, U.Email, U.RegistrationDate, U.DateOfBirth, B.Id, B.Name, B.Description, B.Genre , B.InArchive , B.RegistrationDate
                FROM Reservations AS Res
                LEFT JOIN Users AS U   ON U.Id = Res.UserId
                LEFT JOIN Books AS B   ON B.Id = Res.BookId
                WHERE Res.UserId = @userId
                ORDER BY Res.Id DESC;";
                return (await connection.QueryAsync<Reservation, User, Book, Reservation>(sql, (reservation, user, book) =>
                {
                    reservation.Book = book;
                    reservation.User = user;
                    return reservation;
                }, new { userId = userId })).ToList();
            }
        }

        public async Task<bool> IsBookInReserve(int bookId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string getRowSql = @"
                    Select TOP 1 Res.Id, Res.ReturnDate FROM Reservations AS Res
                    WHERE Res.BookId = @BookId
                    ORDER BY Res.Id DESC;
                ";
                Reservation reservationRow = await connection.QueryFirstOrDefaultAsync<Reservation>(getRowSql, new { BookId = bookId});
                if (reservationRow == null || reservationRow.ReturnDate != default) return false;
                return true;
            }
        }
    }
}
