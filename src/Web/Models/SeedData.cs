using System;
using System.Linq;
using Microsoft.AspNet.Builder;

namespace Web.Models
{
    public static class SeedData
    {
        public static void EnsureSampleData(this IApplicationBuilder app)
        {
            var context = (ApplicationDbContext) app.ApplicationServices.GetService(typeof (ApplicationDbContext));

            if (!context.Libraries.Any())
            {
                context.Libraries.Add(new Library
                {
                    CreatedOn = DateTimeOffset.Now,
                    ModifiedOn = DateTimeOffset.Now,
                    Name = "My First Library"
                });
                context.SaveChanges();
            }
        }
    }
}