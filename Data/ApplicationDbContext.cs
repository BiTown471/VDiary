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
        //public DbSet<VDiary.Models.CourseUser> CourseUser { get; set; }

        // public DbSet<Role> Role { get; set; }
       // public DbSet<User> User { get; set; }
       // public DbSet<Subject> Subjects{ get; set; }


       protected override void OnModelCreating(ModelBuilder modelBuilder)
       {
           /*modelBuilder.Entity<CourseUser>()
               .HasKey(cu => new {cu.CourseId, cu.UserId});

           modelBuilder.Entity<CourseUser>()
               .HasOne(cu => cu.Course)
               .WithMany(c => c.UsersList)
               .HasForeignKey(cu => cu.CourseId);

           modelBuilder.Entity<CourseUser>()
               .HasOne(cu => cu.User)
               .WithMany(c => c.CourseList)
               .HasForeignKey(cu => cu.UserId);*/

           //modelBuilder.Entity<Course>()
           //    .HasMany(c => c.UsersList)
           //    .WithMany(c => c.CourseList)
           //    .UsingEntity(j => j.ToTable("CourseUser"));
       }
    }
}
