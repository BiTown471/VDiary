using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace VDiary.Models.Dtos
{
    public class MarksDto
    {
        public string Subjest { get; set; }
        public List<string> Grades { get; set; }
    }
}
