using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
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
                    Id = 0,
                    Surname = "Admin",
                    FirstName= "Admin",
                    Password = "password",
                    Email = "admin@admin.com",
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

                },
                new User()
            {
                Id = 0,
                Surname = "Jan",
                FirstName = "Nowak",
                Password = "password",
                Email = "jan@nowak.com",
                AlbumNumber = "s345634",
                LastLoggedIn = new DateTime(0001, 01, 01, 01, 01, 01),
                DateCreated = new DateTime(0001, 01, 01, 01, 01, 01),
                AccountExpiryDays = 0,
                MaxLoginAttemps = 0,
                FilledLoginAtemps = 0,
                IsDeleted = false,
                Signature = "",
                DateResetRequest = new DateTime(0001, 01, 01, 01, 01, 01),
                RoleId = 2

            },

            new User()
            {
                Id = 0,
                Surname = "Lewandowski",
                FirstName = "Robert",
                Password = "password",
                Email = "robercik@lewy.com",
                AlbumNumber = "s348934",
                LastLoggedIn = new DateTime(0001, 01, 01, 01, 01, 01),
                DateCreated = new DateTime(0001, 01, 01, 01, 01, 01),
                AccountExpiryDays = 0,
                MaxLoginAttemps = 0,
                FilledLoginAtemps = 0,
                IsDeleted = false,
                Signature = "",
                DateResetRequest = new DateTime(0001, 01, 01, 01, 01, 01),
                RoleId = 2

            },
            new User()
            {
                Id = 0,
                Surname = "Pudzianowski",
                FirstName = "Mariusz",
                Password = "password",
                Email = "mariusz@pudzian.com",
                AlbumNumber = "s125634",
                LastLoggedIn = new DateTime(0001, 01, 01, 01, 01, 01),
                DateCreated = new DateTime(0001, 01, 01, 01, 01, 01),
                AccountExpiryDays = 0,
                MaxLoginAttemps = 0,
                FilledLoginAtemps = 0,
                IsDeleted = false,
                Signature = "",
                DateResetRequest = new DateTime(0001, 01, 01, 01, 01, 01),
                RoleId = 2

            },
            new User()
            {
                Id = 0,
                Surname = "Odziomek",
                FirstName = "Monika",
                Password = "password",
                Email = "monia@odzi.com",
                AlbumNumber = "s125634",
                LastLoggedIn = new DateTime(0001, 01, 01, 01, 01, 01),
                DateCreated = new DateTime(0001, 01, 01, 01, 01, 01),
                AccountExpiryDays = 0,
                MaxLoginAttemps = 0,
                FilledLoginAtemps = 0,
                IsDeleted = false,
                Signature = "",
                DateResetRequest = new DateTime(0001, 01, 01, 01, 01, 01),
                RoleId = 2

            },
            new User()
            {
                Id = 0,
                Surname = "Walaszek",
                FirstName = "Adrian",
                Password = "password",
                Email = "adi@walo.com",
                AlbumNumber = "s125764",
                LastLoggedIn = new DateTime(0001, 01, 01, 01, 01, 01),
                DateCreated = new DateTime(0001, 01, 01, 01, 01, 01),
                AccountExpiryDays = 0,
                MaxLoginAttemps = 0,
                FilledLoginAtemps = 0,
                IsDeleted = false,
                Signature = "",
                DateResetRequest = new DateTime(0001, 01, 01, 01, 01, 01),
                RoleId = 3

            },
            new User()
            {
                Id = 0,
                Surname = "Kapela",
                FirstName = "Jan",
                Password = "password",
                Email = "jas@kapela.com",
                AlbumNumber = "s120934",
                LastLoggedIn = new DateTime(0001, 01, 01, 01, 01, 01),
                DateCreated = new DateTime(0001, 01, 01, 01, 01, 01),
                AccountExpiryDays = 0,
                MaxLoginAttemps = 0,
                FilledLoginAtemps = 0,
                IsDeleted = false,
                Signature = "",
                DateResetRequest = new DateTime(0001, 01, 01, 01, 01, 01),
                RoleId = 3

            },
            new User()
            {
                Id = 0,
                Surname = "Małysz",
                FirstName = "Adam",
                Password = "password",
                Email = "adam@malysz.com",
                AlbumNumber = "s190934",
                LastLoggedIn = new DateTime(0001, 01, 01, 01, 01, 01),
                DateCreated = new DateTime(0001, 01, 01, 01, 01, 01),
                AccountExpiryDays = 0,
                MaxLoginAttemps = 0,
                FilledLoginAtemps = 0,
                IsDeleted = false,
                Signature = "",
                DateResetRequest = new DateTime(0001, 01, 01, 01, 01, 01),
                RoleId = 3

            },
            new User()
            {
                Id = 0,
                Surname = "Stasko",
                FirstName = "Maja",
                Password = "password",
                Email = "maja@stasko.com",
                AlbumNumber = "s000934",
                LastLoggedIn = new DateTime(0001, 01, 01, 01, 01, 01),
                DateCreated = new DateTime(0001, 01, 01, 01, 01, 01),
                AccountExpiryDays = 0,
                MaxLoginAttemps = 0,
                FilledLoginAtemps = 0,
                IsDeleted = false,
                Signature = "",
                DateResetRequest = new DateTime(0001, 01, 01, 01, 01, 01),
                RoleId = 3

            },
        };
            return users;

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
    }
}
