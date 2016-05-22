using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Web.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Document> Documents { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Library> Libraries { get; set; }
        public DbSet<StatusType> StatusTypes { get; set; }
        public DbSet<PermissionType> PermissionTypes { get; set; }

        public DbSet<UserLibrary> UserLibraries { get; set; }
        public DbSet<UserDocument> UserDocuments { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // library document: one-to-many

            builder.Entity<LibraryDocument>()
                .HasKey(ld => new {ld.LibraryId, ld.DocumentId});

            builder.Entity<LibraryDocument>()
                .HasOne(ld => ld.Library)
                .WithMany(l => l.Documents)
                .HasForeignKey(ld => ld.LibraryId);

            builder.Entity<LibraryDocument>()
                .HasOne(ld => ld.Document)
                .WithMany(d => d.Libraries)
                .HasForeignKey(ld => ld.DocumentId);

            // user document permissions: one-to-many

            builder.Entity<UserDocument>()
                .HasKey(udp => new { udp.DocumentId, udp.ApplicationUserId });

            builder.Entity<UserDocument>()
                .HasOne(udp => udp.ApplicationUser)
                .WithMany(au => au.DocumentAccessList)
                .HasForeignKey(udp => udp.ApplicationUserId);

            builder.Entity<UserDocument>()
                .HasOne(udp => udp.Document)
                .WithMany(d => d.UserPermissions)
                .HasForeignKey(udp => udp.DocumentId);

            // user library permissions: one-to-many

            builder.Entity<UserLibrary>()
                .HasKey(ulp => new { ulp.LibraryId, ulp.ApplicationUserId });

            builder.Entity<UserLibrary>()
                .HasOne(ulp => ulp.ApplicationUser)
                .WithMany(au => au.LibraryAccessList)
                .HasForeignKey(ulp => ulp.ApplicationUserId);

            builder.Entity<UserLibrary>()
                .HasOne(ulp => ulp.Library)
                .WithMany(l => l.UserPermissions)
                .HasForeignKey(ulp => ulp.LibraryId);

            // lookups

            builder.Entity<PermissionType>()
                .Property(pt => pt.Id)
                .ValueGeneratedNever();

            builder.Entity<StatusType>()
                .Property(st => st.Id)
                .ValueGeneratedNever();
        }
    }
}