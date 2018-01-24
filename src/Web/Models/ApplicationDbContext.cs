using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Web.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Document> Documents { get; set; }

        public DbSet<DataFile> DataFiles { get; set; }

        public DbSet<Revision> Revisions { get; set; }
        public DbSet<PendingRevision> PendingRevisions { get; set; }
        public DbSet<DeletedRevision> DeletedRevisions { get; set; }
        public DbSet<PublishedRevision> PublishedRevisions { get; set; }

        public DbSet<DistributionGroup> DistributionGroups { get; set; }
        public DbSet<DistributionGroupType> DistributionGroupTypes { get; set; }
        public DbSet<NamedDistribution> NamedDistributions { get; set; }
        public DbSet<StatusType> StatusTypes { get; set; }
        public DbSet<UnnamedDistribution> UnnamedDistributions { get; set; }
        public DbSet<PermissionType> PermissionTypes { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Revision>()
                .HasDiscriminator<int>("Status")
                .HasValue<PendingRevision>((int)Models.StatusTypes.Pending)
                .HasValue<PublishedRevision>((int)Models.StatusTypes.Active)
                .HasValue<DeletedRevision>((int)Models.StatusTypes.Deleted);

            // distribution

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

            // file

            builder.Entity<Revision>()
                .HasKey(f => new { f.DocumentId, f.VersionNum });

            // nameddistribution

            builder.Entity<NamedDistribution>()
                .HasKey(nd => new { nd.DistributionGroupId, nd.ApplicationUserId });

            // unnameddistribution

            builder.Entity<UnnamedDistribution>()
                .HasOne(ud => ud.DistributionGroup)
                .WithMany()
                .HasForeignKey(ud => ud.DistributionGroupId);

            // lookups

            builder.Entity<DistributionGroupType>()
                .Property(dgt => dgt.Id)
                .ValueGeneratedNever();

            builder.Entity<PermissionType>()
                .Property(pt => pt.Id)
                .ValueGeneratedNever();

            builder.Entity<StatusType>()
                .Property(st => st.Id)
                .ValueGeneratedNever();
        }
    }
}