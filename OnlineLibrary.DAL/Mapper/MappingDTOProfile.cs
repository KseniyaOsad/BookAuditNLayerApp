using AutoMapper;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.DAL.DTO;

namespace OnlineLibrary.DAL.Mapper
{
    public class MappingDTOProfile : Profile
    {
        public MappingDTOProfile()
        {
            CreateMap<Reservation, ReservationDTO>();
            CreateMap<Book, BookDTO>();
        }
    }
}
