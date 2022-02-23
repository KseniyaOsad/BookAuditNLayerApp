using Dapper;
using DapperParameters;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using OnlineLibrary.Common.Connection;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.DAL.DTO;
using OnlineLibrary.DAL.Interfaces;
using System.Collections.Generic;
using System.Data;
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
                await connection.ExecuteAsync("sp_CloseReservation",
                    new { bookId = reservation.Book.Id, userId = reservation.User.Id },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<Reservation> GetBookReservationLastRow(int bookId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryFirstOrDefaultAsync<Reservation>("sp_GetLastBookReservation",
                    new { bookId },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task CreateReservationAsync(Reservation reservation)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                reservation.Id = await connection.ExecuteScalarAsync<int>("sp_CreateReservation",
                   new { bookId = reservation.Book.Id, userId = reservation.User.Id },
                   commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<List<Reservation>> GetAllReservationsAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return (await connection.QueryAsync<Reservation, User, Book, Reservation>("sp_GetAllReservations", (reservation, user, book) =>
                {
                    reservation.Book = book;
                    reservation.User = user;
                    return reservation;
                },
                commandType: CommandType.StoredProcedure)
                ).ToList();
            }
        }

        public async Task<List<Reservation>> GetBookReservationHistoryAsync(int bookId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return (await connection.QueryAsync<Reservation, User, Book, Reservation>("sp_GetBookReservations", (reservation, user, book) =>
                {
                    reservation.Book = book;
                    reservation.User = user;
                    return reservation;
                },
                new { bookId },
                commandType: CommandType.StoredProcedure)
                ).ToList();
            }
        }

        public async Task<List<Reservation>> GetUserReservationHistoryAsync(int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return (await connection.QueryAsync<Reservation, User, Book, Reservation>("sp_GetUserReservation", (reservation, user, book) =>
                {
                    reservation.Book = book;
                    reservation.User = user;
                    return reservation;
                },
                new { userId },
                commandType: CommandType.StoredProcedure)
                ).ToList();
            }
        }

        public async Task UpdateBookReservationsAsync(List<UpdateReservations> updateReservations)
        {
            List<ReservationDTO> deleteRes = updateReservations.Where(r => r.Status == DTO.Enums.UpdateStatus.Delete).Select(r => new ReservationDTO(r.Reservation)).ToList();
            List<ReservationDTO> updateRes = updateReservations.Where(r => r.Status == DTO.Enums.UpdateStatus.Update).Select(r => new ReservationDTO(r.Reservation)).ToList();
            List<ReservationDTO> createRes = updateReservations.Where(r => r.Status == DTO.Enums.UpdateStatus.Create).Select(r => new ReservationDTO(r.Reservation)).ToList();
            var parameters = new DynamicParameters();
            parameters.AddTable("@delete", "t_Reservation", deleteRes);
            parameters.AddTable("@update", "t_Reservation", updateRes);
            parameters.AddTable("@create", "t_Reservation", createRes);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync("sp_UpdateBookReservations",
                                parameters,
                                commandType: CommandType.StoredProcedure);
            }
        }
    }
}
