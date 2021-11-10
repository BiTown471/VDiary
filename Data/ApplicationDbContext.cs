﻿using Microsoft.EntityFrameworkCore;
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
       // public DbSet<Role> Role { get; set; }
       // public DbSet<User> User { get; set; }
       // public DbSet<Subject> Subjects{ get; set; }


    }
}
