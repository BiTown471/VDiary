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
        public string Surname { get; set; }
        [Required]
        public string FirstName { get; set; }
        [MinLength(6)]
        public string Password { get; set; }
        [Display(Name = "Email")]
        [Required]
        [MaxLength(50, ErrorMessage = "Max 50 char")]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        public string AlbumNumber { get; set; }
        public DateTime LastLoggedIn { get; set; }
        public DateTime DateCreated { get; set; }
        public int AccountExpiryDays { get; set; }
        public int MaxLoginAttemps { get; set; }
        public int FilledLoginAtemps { get; set; }
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
        public ICollection<Course> Courses { get; set; }
        public List<CourseUser> CourseUsers { get; set; }
    }
}
