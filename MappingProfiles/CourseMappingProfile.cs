using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using VDiary.Models;
using VDiary.Models.Dtos;

namespace VDiary.MappingProfiles
{
    public class CourseMappingProfile : Profile
    {
        public CourseMappingProfile()
        {
            CreateMap<Course, CourseDto>()
                .ForMember(c => c.Lecturer, u => u.MapFrom(c => c.Lecturer.FullName))
                .ForMember(c => c.Subject, u => u.MapFrom(c => c.Subject.Name));


        }
    }
}
