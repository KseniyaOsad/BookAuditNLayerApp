using AutoMapper;
using OnlineLibrary.API.Model;
using OnlineLibrary.Common.Entities;
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
        }
    }
}
