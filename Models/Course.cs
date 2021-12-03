using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VDiary.Models
{
    public class Course
    {
        public int Id { get; set; }

        [DisplayName("Name of course")]
        public int SubjectId { get; set; } //Name of course
        public virtual Subject Subject { get; set; }
        public int LecturerId { get; set; }
        public DateTime Time { get; set; }

        [MaxLength(10)]
        public string Venue { get; set; } // place
        public string GroupName { get; set; }
        public bool Active { get; set; }

        public ICollection<User> Users{ get; set; }
        public List<CourseUser> CourseUsers{ get; set; }
    }
}
