using AutoMapper;
using OnlineLibrary.API.Model;
using OnlineLibrary.Common.DBEntities;
using System;
using System.Linq;

namespace OnlineLibrary.API.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateBook, Book>()
                .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.Authors.Select(x => new Author() { Id = x })))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(x => new Tag() { Id = x })));
            CreateMap<ReservationModel, Reservation>()
                .ForMember(dest => dest.Book, opt => opt.MapFrom(src => new Book() { Id = src.BookId }))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => new User() { Id = src.UserId }));
            CreateMap<CreateUser, User>()
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => new DateTime(src.Year, src.Month, src.Day)));
        }
    }
}
