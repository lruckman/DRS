using System;
using System.Linq;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Identity;

namespace Web.Models
{
    public static class SeedData
    {
        public static void EnsureSampleData(this IApplicationBuilder app)
        {
            var context = (ApplicationDbContext) app.ApplicationServices
                .GetService(typeof (ApplicationDbContext));

            var userManager = (UserManager<ApplicationUser>) app.ApplicationServices
                .GetService(typeof (UserManager<ApplicationUser>));

            if (!context.Libraries.Any())
            {
                context.Libraries.Add(new Library
                {
                    CreatedOn = DateTimeOffset.Now,
                    ModifiedOn = DateTimeOffset.Now,
                    Name = "Private"
                });
                context.SaveChanges();
            }

            // add a test user

            if (!context.Users.Any())
            {
                var defaultUser = new ApplicationUser
                {
                    UserName = "test@localhost.com",
                    Email = "test@localhost.com"
                };

                userManager
                    .CreateAsync(defaultUser, "P@ssword1")
                    .Wait();
            }
        }
    }
}