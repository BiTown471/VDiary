using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VDiary.Models
{
    public class Course
    {
        public int Id { get; set; }
        [DisplayName("Name of course")]
        [Required]
        public int SubjectId { get; set; } //Name of course
        public virtual Subject Subject { get; set; }
        [Required]
        public int LecturerId { get; set; }
        public virtual User Lecturer { get; set; }
        [Required]
        public DateTime Time { get; set; }
        [MaxLength(10)]
        public string Venue { get; set; } // place
        public string GroupName { get; set; }
        public bool Active { get; set; }
    }
}
