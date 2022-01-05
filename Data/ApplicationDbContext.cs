using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using VDiary.Models;

namespace VDiary.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<VDiary.Models.Role> Role { get; set; }
        public DbSet<VDiary.Models.Subject> Subject { get; set; }
        public DbSet<VDiary.Models.User> User { get; set; }
        public DbSet<VDiary.Models.Course> Course { get; set; }
        public DbSet<VDiary.Models.SubjectUser> SubjectUser { get; set; }
        public DbSet<VDiary.Models.Grade> Grade { get; set; }
        public DbSet<VDiary.Models.Presence> Presence { get; set; }
        
    }
}
