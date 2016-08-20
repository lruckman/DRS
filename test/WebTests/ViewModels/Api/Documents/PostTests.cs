using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Web.Engine;
using Web.Engine.Services;
using Web.Models;
using Web.ViewModels.Api.Documents;
using Xunit;

namespace WebTests.ViewModels.Api.Documents
{
    public class PostTests : BaseViewModelTest
    {
        private TestBmpFormFile CreateTestFile()
        {
            return new TestBmpFormFile();
        }

        private FileDecoder.IFileInfo CreateFileInfo(TestBmpFormFile file)
        {
            var fileInfo = new Mock<FileDecoder.IFileInfo>();

            fileInfo
                .Setup(x => x.Content)
                .Returns(file.FileName);

            fileInfo
                .Setup(x => x.Abstract)
                .Returns(file.FileName);

            fileInfo
                .Setup(x => x.Buffer)
                .Returns(file.Buffer);

            fileInfo
                .Setup(x => x.Length)
                .Returns(file.Length);

            fileInfo
                .Setup(x => x.PageCount)
                .Returns(1);

            fileInfo
                .Setup(x => x.CreateThumbnail(It.IsAny<Size>(), It.IsAny<int>()))
                .Returns(file.Buffer);

            return fileInfo.Object;
        }

        [Fact]
        public async Task CommandHandler_CreatesDocument()
        {
            // the setup

            var db = CreateDbContext();

            var userContext = new Mock<IUserContext>();

            userContext
                .Setup(x => x.UserId)
                .Returns("TestUser");

            var fileMover = new Mock<IFileMover>();

            fileMover
                .Setup(x => x.Move(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .Returns(Task.FromResult(@"\\127.0.0.1\nofile"));

            var fileEncryptor = new Mock<IFileEncryptor>();

            fileEncryptor
                .Setup(x => x.Encrypt(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .Returns(new byte[] {});

            var testFile = CreateTestFile();
            var fileInfo = CreateFileInfo(testFile);

            var fileDecoder = new Mock<IFileDecoder>();

            fileDecoder
                .Setup(x => x.Decode(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Returns(fileInfo);

            var commandHandler = new Post.CommandHandler(db, userContext.Object, fileMover.Object,
                fileDecoder.Object, fileEncryptor.Object);

            var command = new Post.Command
            {
                File = testFile
            };

            var result = await commandHandler.Handle(command);

            // the test

            Assert.NotNull(result);
            Assert.True(result.Status == Post.Result.StatusTypes.Success);
            Assert.True(result.DocumentId == 1);

            var document = await db.Documents
                .Include(d => d.Content)
                .Include(d => d.Files)
                .SingleOrDefaultAsync(d => d.Id == result.DocumentId);

            Assert.NotNull(document);
            Assert.True(document.Status == StatusTypes.Active);

            Assert.Equal(document.Abstract, fileInfo.Abstract);

            Assert.NotNull(document.Content);
            Assert.NotNull(document.Content?.Content);
            Assert.Equal(document.Content?.Content, fileInfo.Content);

            Assert.True(document.Files.Count == 1);

            var file = document.Files.First();

            Assert.NotNull(file.Extension);
            Assert.Equal(file.Extension, Path.GetExtension(testFile.FileName));

            Assert.NotNull(file.Path);
            Assert.NotNull(file.ThumbnailPath);
            Assert.NotNull(file.Key);
            
        }
    }
}