using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Web.Engine;
using Web.Models;
using Web.ViewModels.Api.Documents;
using Xunit;
using File = System.IO.File;

namespace WebTests.ViewModels.Api.Documents
{
    public class PostTests : BaseViewModelTest
    {
        [Fact]
        public async Task CommandHandler_CreatesDocument()
        {
            // the setup

            var db = CreateDbContext();

            var userContext = new Mock<IUserContext>();

            userContext
                .Setup(x => x.UserId)
                .Returns(""); //todo: fill

            var settings = new Mock<IOptions<DRSConfig>>();

            settings
                .Setup(x => x.Value)
                .Returns(new DRSConfig
                {
                    BasePath = "", //todo: fill
                    DocumentPath = "", //todo: fill
                    TessDataPath = "" //todo: fill
                });

            var commandHandler = new Post.CommandHandler(db, settings.Object, userContext.Object);
            var command = new Post.Command {File = null};  //todo: fill

            var result = await commandHandler.Handle(command);

            // the test

            Assert.NotNull(result);
            Assert.True(result.Status == Post.Result.StatusTypes.Success);
            Assert.True(result.DocumentId == 0);

            var document = await db.Documents
                .Include(d => d.Content)
                .Include(d => d.Files)
                .SingleOrDefaultAsync(d => d.Id == result.DocumentId);

            Assert.NotNull(document);
            Assert.True(document.Status == StatusTypes.Active);

            Assert.NotNull(document.Content);
            Assert.NotNull(document.Content.Content);

            Assert.True(document.Files.Count == 1);

            var file = document.Files.First();

            Assert.NotNull(file.Extension);
            Assert.NotNull(file.Path);
            Assert.NotNull(file.ThumbnailPath);
            Assert.NotNull(file.Key);

            Assert.True(File.Exists(file.Path));
            Assert.True(File.Exists(file.ThumbnailPath));

            //todo: test data protection for document/thumbnail
        }
    }
}