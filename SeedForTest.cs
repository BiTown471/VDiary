using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VDiary.Data;
using VDiary.Models;

namespace VDiary
{
    public class SeedForUser
    {
        private readonly ApplicationDbContext _dbContext;
        
        public SeedForUser(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public void SeederAll(int size)
        {
            var rand = new Random();
            var listUsers = new List<User>();
            for (int i = 0; i < size; i++)
            {
                var u = new User()
                {
                    Id = 0,
                    Surname = "Test" + rand.Next(0, Int32.MaxValue),
                    FirstName = "Maja",
                    Password = "password",
                    Email = $"test{rand.Next(0, 999999)}@test.com",
                    AlbumNumber = "s" + rand.Next(0, 999999),
                    LastLoggedIn = new DateTime(0001, 01, 01, 01, 01, 01),
                    DateCreated = new DateTime(0001, 01, 01, 01, 01, 01),
                    AccountExpiryDays = 0,
                    MaxLoginAttemps = 0,
                    FilledLoginAtemps = 0,
                    IsDeleted = false,
                    Signature = "",
                    DateResetRequest = new DateTime(0001, 01, 01, 01, 01, 01),
                    RoleId = 3
                };
                listUsers.Add(u);

            }
            _dbContext.User.AddRange(listUsers);
            _dbContext.SaveChanges();

        }
    }
}
