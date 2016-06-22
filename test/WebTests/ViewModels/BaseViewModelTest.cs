using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace WebTests.ViewModels
{
    public class BaseViewModelTest
    {
        public ApplicationDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseInMemoryDatabase();

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}