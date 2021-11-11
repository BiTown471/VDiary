using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDiary.Models
{
    public class CourseUser
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int UserId { get; set; }
        public User User{ get; set; }
    }
}
