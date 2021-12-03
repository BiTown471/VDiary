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
        public DbSet<VDiary.Models.CourseUser> CourseUser { get; set; }

        // public DbSet<Role> Role { get; set; }
       // public DbSet<User> User { get; set; }
       // public DbSet<Subject> Subjects{ get; set; }


       protected override void OnModelCreating(ModelBuilder modelBuilder)
       {


            //modelBuilder.Entity<Course>()
            //    .HasMany(c => c.Users)
            //    .WithMany(c => c.Courses)
            //    .UsingEntity(j => j.ToTable("CourseUser"));

            //modelBuilder.Entity<Course>()
            //    .HasMany(c => c.Users)
            //    .WithMany(c => c.Courses)
            //    .UsingEntity<Dictionary<string, object>>(
            //        "CourseUser", 
            //        j => j
            //            .HasOne<User>()
            //            .WithMany()
            //            .HasForeignKey("UserId")
            //            .HasConstraintName("FK_CourseUser_Users_UserId")
            //            .OnDelete(DeleteBehavior.Cascade),
            //        j => j
            //            .HasOne<Course>()
            //            .WithMany()
            //            .HasForeignKey("CourseId")
            //            .HasConstraintName("FK_CourseUser_Courses_CourseId")
            //            .OnDelete(DeleteBehavior.ClientCascade))
            //        ;

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Users)
                .WithMany(c => c.Courses)
                .UsingEntity<CourseUser>(
                    j => j
                        .HasOne(cu => cu.User)
                        .WithMany(t => t.CourseUsers)
                        .HasForeignKey(cu => cu.UserId),
                    j => j
                        .HasOne(cu => cu.Course)
                        .WithMany(p => p.CourseUsers)
                        .HasForeignKey(cu => cu.CourseId),
                    j =>
                    {
                        j.HasKey(t => new { t.Id});
                    }
                    );
        }
    }
}
