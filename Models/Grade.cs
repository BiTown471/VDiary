using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VDiary.Models
{
    public class Grade
    {
        public int Id { get; set; }
        [MaxLength(1)]
        public string GradeMark { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Date")]
        [DisplayFormat(DataFormatString = "0:{dd/mm/yyyy}")]
        public DateTime Date { get; set; } = DateTime.Now;
        public string Description { get; set; }
        public int CreatedBy { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int SubjectId { get; set; }
        public virtual  Subject Subject{ get; set; }

    }
}
