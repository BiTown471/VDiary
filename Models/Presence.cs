using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using VDiary.Migrations;

namespace VDiary.Models
{
    public class Presence
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public DateTime Time { get; set; }
        public int CourseId { get; set; }
        public int UserId { get; set; }
        public virtual Course Course { get; set; }
        public virtual User User { get; set; }

    }
}
