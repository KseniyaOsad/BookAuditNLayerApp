using AutoMapper;
using OnlineLibrary.API.Model;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLibrary.API.Mapper
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateBook, Book>().ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.Authors.Select(x => new Author() { Id = x })));
        }
    }
}
