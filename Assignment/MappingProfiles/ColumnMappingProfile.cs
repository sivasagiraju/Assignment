using Assignment.DTOs;
using Assignment.Repository.Collections;
using AutoMapper;
using System.Diagnostics.CodeAnalysis;

namespace Assignment.MappingProfiles
{
    [ExcludeFromCodeCoverage]
    public class ColumnMappingProfile : Profile
    {
        public ColumnMappingProfile()
        {
            CreateMap<ColumnItem, ColumnDto>().ReverseMap();
            CreateMap<CreateColumnDto, ColumnItem>();
        }
    }
}
