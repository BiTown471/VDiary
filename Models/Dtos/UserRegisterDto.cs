using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VDiary.Models.Dtos
{
    public class UserRegisterDto
    {
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
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MaxLength(10)]
        public string AlbumNumber { get; set; }
    }
}
