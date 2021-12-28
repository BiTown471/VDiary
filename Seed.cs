using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VDiary.Data;
using VDiary.Models;

namespace VDiary
{
    public class Seed
    {
        private readonly ApplicationDbContext _dbContext;

        public Seed(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void SeederAll()
        {
            if (_dbContext.Database.CanConnect())
            {
                if (!_dbContext.Role.Any())
                {
                    var role = GetRole();
                    _dbContext.Role.AddRange(role);
                    _dbContext.SaveChanges();
                }

                if (!_dbContext.User.Any())
                {
                    var users = GetUser();
                    _dbContext.User.AddRange(users);
                    _dbContext.SaveChanges();
                }
            }
        }

        private IEnumerable<Role> GetRole()
        {
            var role = new List<Role>()
            {
                new Role()
                {
                    Name = "Admin",
                },
                new Role()
                {
                    Name = "Lecturer",
                },
                new Role()
                {
                    Name = "Student",
                }
            };

            return role;

        }
        private IEnumerable<User> GetUser()
        {
            var users = new List<User>()
            {
                new User()
                {
                    
                    Surname = "Admin",
                    FirstName= "Admin",
                    Password = "password",
                    Email = "Admin@admin.com",
                    AlbumNumber = "A",
                    LastLoggedIn = new DateTime(0001,01,01,01,01,01),
                    DateCreated= new DateTime(0001,01,01,01,01,01),
                    AccountExpiryDays = 0,
                    MaxLoginAttemps = 0,
                    FilledLoginAtemps= 0,
                    IsDeleted = false,
                    Signature = "",
                    DateResetRequest= new DateTime(0001,01,01,01,01,01),
                    RoleId = 1 

                }
            };

            return users;

        }
    }
}
