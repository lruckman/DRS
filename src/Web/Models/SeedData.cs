﻿using System;
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

            // add a test user

            var defaultUser = new ApplicationUser
            {
                UserName = "test@localhost.com",
                Email = "test@localhost.com"
            };

            if (!context.Users.Any())
            {
                userManager
                    .CreateAsync(defaultUser, "P@ssword1")
                    .Wait();
            }

            // add a library

            if (!context.Libraries.Any())
            {
                var personalLibrary = new Library
                {
                    CreatedByUserId = userManager.GetUserIdAsync(defaultUser).Result,
                    CreatedOn = DateTimeOffset.Now,
                    ModifiedOn = DateTimeOffset.Now,
                    Name = "Private"
                };

                context.Libraries.Add(personalLibrary);
                context.SaveChanges();

                // give the default user full access to their private cabinet

                defaultUser.LibraryAccessList.Add(new UserLibrary
                {
                    LibraryId = personalLibrary.Id,
                    Permissions = PermissionTypes.Full
                });
            }

            // Permission Types

            if (!context.PermissionTypes.Any())
            {
                foreach (PermissionTypes val in Enum.GetValues(typeof(PermissionTypes)))
                {
                    if (val != PermissionTypes.Full)
                    {
                        context.PermissionTypes.Add(new PermissionType
                        {
                            Id = (int) val,
                            Name = val.ToString()
                        });
                    }
                }
                context.SaveChanges();
            }

            // Statuses

            if (!context.StatusTypes.Any())
            {
                foreach (StatusTypes val in Enum.GetValues(typeof(StatusTypes)))
                {
                    context.StatusTypes.Add(new StatusType
                    {
                        Id = (int)val,
                        Name = val.ToString()
                    });
                }
                context.SaveChanges();
            }
        }
    }
}