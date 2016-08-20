using System;
using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace WebTests.ViewModels
{
    public class BaseViewModelTest
    {
        protected ApplicationDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseInMemoryDatabase();

            var db = new ApplicationDbContext(optionsBuilder.Options);

            db.Libraries.Add(new Library
            {
                CreatedByUserId = "dbo",
                CreatedOn = DateTimeOffset.Now,
                ModifiedOn = DateTimeOffset.Now,
                Name = "Test Library",
                Status = StatusTypes.Active
            });

            db.SaveChanges();

            return db;
        }
    }
}