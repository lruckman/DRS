using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;

namespace Web.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Document> Documents { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Library> Libraries { get; set; }
        public DbSet<StatusType> StatusTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<LibraryDocument>()
                .HasKey(t => new {t.LibraryId, t.DocumentId});

            builder.Entity<LibraryDocument>()
                .HasOne(pt => pt.Library)
                .WithMany(p => p.Documents)
                .HasForeignKey(pt => pt.LibraryId);

            builder.Entity<LibraryDocument>()
                .HasOne(pt => pt.Document)
                .WithMany(p => p.Libraries)
                .HasForeignKey(pt => pt.DocumentId);

            builder.Entity<StatusType>()
                .Property(x => x.Id)
                .ValueGeneratedNever();
        }
    }
}