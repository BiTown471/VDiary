using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDiary.Models.Dtos
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string LecturerName { get; set; }
        public DateTime Time { get; set; }
        public string Venue { get; set; } // place
        public string GroupName { get; set; }
        public bool Active { get; set; }
    }
}
