using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Web.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Document> Documents { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<DistributionGroup> DistributionGroups { get; set; }
        public DbSet<StatusType> StatusTypes { get; set; }
        public DbSet<PermissionType> PermissionTypes { get; set; }

        public DbSet<DistributionRecipient> DistributionRecipients { get; set; }
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // library document: one-to-many

            builder.Entity<Distribution>()
                .HasKey(dd => new { dd.DistributionGroupId, dd.DocumentId });

            builder.Entity<Distribution>()
                .HasOne(dd => dd.DistributionGroup)
                .WithMany(dg => dg.Distributions)
                .HasForeignKey(dd => dd.DistributionGroupId);

            builder.Entity<Distribution>()
                .HasOne(ld => ld.Document)
                .WithMany(d => d.Distributions)
                .HasForeignKey(ld => ld.DocumentId);

            // user library permissions: one-to-many

            builder.Entity<DistributionRecipient>()
                .HasKey(dr => new { dr.DistributionGroupId, dr.ApplicationUserId });

            builder.Entity<DistributionRecipient>()
                .HasOne(dr => dr.ApplicationUser)
                .WithMany(au => au.LibraryAccessList)
                .HasForeignKey(dr => dr.ApplicationUserId);

            builder.Entity<DistributionRecipient>()
                .HasOne(dr => dr.DistributionGroup)
                .WithMany(dg => dg.Recipients)
                .HasForeignKey(dr => dr.DistributionGroupId);

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