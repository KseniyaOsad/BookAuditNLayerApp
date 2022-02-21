using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineLibrary.API.Model;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.DBEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLibrary.API.Controllers
{
    [Route("api/reservations")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        private readonly ILogger<ReservationController> _logger;

        private readonly IMapper _mapper;

        public ReservationController(IReservationService iReservation, IMapper mapper, ILogger<ReservationController> logger)
        {
            _reservationService = iReservation;
            _logger = logger;
            _mapper = mapper;
        }

        // POST:  api/reservations
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ReservationModel reservationModel)
        {
            Reservation reservation = _mapper.Map<ReservationModel, Reservation>(reservationModel);
            _logger.LogInformation("Map CreateReservation to Reservation.");

            int id = await _reservationService.CreateReservationAsync(reservation);
            _logger.LogInformation($"New book reservation. reservation ID = {id}");
            return Ok(id);
        }

        // POST:  api/reservations/close-reserve
        [HttpPost("close-reserve")]
        public async Task<IActionResult> CloseReserveAsync([FromBody] ReservationModel reservationModel)
        {
            Reservation reservation = _mapper.Map<ReservationModel, Reservation>(reservationModel);
            _logger.LogInformation("Map CreateReservation to Reservation.");

            await _reservationService.CloseReservationAsync(reservation);
            _logger.LogInformation($"Close reservation. Book ID = {reservationModel.BookId}. User ID = {reservationModel.UserId}");
            return Ok(await _reservationService.GetAllReservationsAsync());
        }

        // Get: api/reservations
        [HttpGet]
        public async Task<IActionResult> GetAllReservationsAsync()
        {
            List<Reservation> reservations = await _reservationService.GetAllReservationsAsync();
            _logger.LogInformation($"Getting all reservations. Reservations count = {reservations?.Count}.");
            return Ok(reservations);
        }
    }
}
