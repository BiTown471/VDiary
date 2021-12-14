using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VDiary.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(55)]
        public string Surname { get; set; }
        [Required]
        [MaxLength(55)]
        public string FirstName { get; set; }
        [MinLength(6)]
        [MaxLength(16)]
        public string Password { get; set; }
        [Display(Name = "Email")]
        [Required]
        [MaxLength(50, ErrorMessage = "Max 50 char")]
        [EmailAddress ]
        public string Email { get; set; }
        [Required]
        [MaxLength(10)]
        public string AlbumNumber { get; set; }
        public DateTime LastLoggedIn { get; set; }
        public DateTime DateCreated { get; set; }
        public int AccountExpiryDays { get; set; }
        public int MaxLoginAttemps { get; set; } = 5;
        public int FilledLoginAtemps { get; set; } = 0;
        public DateTime DateFilledLoginAtemps { get; set; }
        public bool IsDeleted { get; set; }
        [Range(1, Int32.MaxValue)]
        public string Signature { get; set; }
        public DateTime DateResetRequest { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return FirstName + " " + Surname;
            }
        }

        [Display(Name = "Role")]
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
        //public ICollection<Course> Courses { get; set; }
        //public List<CourseUser> CourseUsers { get; set; }
        public ICollection<CourseUser> CourseUsers { get; set; }
                      
    }
}
