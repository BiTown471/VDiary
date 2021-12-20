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
                .ForMember(c => c.Subject, u => u.MapFrom(c => c.Subject.Name))
                .ForMember(c => c.Id, u => u.MapFrom(c => c.Id))
                .ForMember(c => c.LecturerName, u => u.MapFrom(c => c.CourseUsers.FirstOrDefault(u => u.UserId==c.LecturerId).User.FullName))
                .ForMember(c => c.GroupName, u => u.MapFrom(c => c.GroupName));


            CreateMap<CourseDto, Course>();
        }
    }
}
