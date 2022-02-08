using Microsoft.EntityFrameworkCore;

namespace VDiary.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<Models.Role> Role { get; set; }
        public DbSet<Models.Subject> Subject { get; set; }
        public DbSet<Models.User> User { get; set; }
        public DbSet<Models.Course> Course { get; set; }
        public DbSet<Models.SubjectUser> SubjectUser { get; set; }
        public DbSet<Models.Grade> Grade { get; set; }
        public DbSet<Models.Presence> Presence { get; set; }
    }
}
