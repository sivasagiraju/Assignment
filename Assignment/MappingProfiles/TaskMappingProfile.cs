using Assignment.DTOs;
using Assignment.Helpers;
using Assignment.Repository.Collections;
using AutoMapper;
using System.Diagnostics.CodeAnalysis;

namespace Assignment.MappingProfiles
{
    [ExcludeFromCodeCoverage]
    public class TaskMappingProfile : Profile
    {
        public TaskMappingProfile()
        {
            CreateMap<TaskItem, TaskDto>().ReverseMap();
            CreateMap<CreateTaskDto, TaskItem>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.ColumnId, opt => opt.MapFrom(src => DefaultColumns.ToDoId));

            CreateMap<UpdateTaskImagesDto, TaskItem>()
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls ?? new List<string>()));

            CreateMap<UpdateTaskFavouriteDto, TaskItem>()
                .ForMember(dest => dest.IsFavourite, opt => opt.MapFrom(src => src.IsFavourite));
        }
    }
}
