﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Web.Models;

namespace Web.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20170123032147_distributions_part_ii")]
    partial class distributions_part_ii
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Web.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Web.Models.DistributionDocument", b =>
                {
                    b.Property<int>("DistributionGroupId");

                    b.Property<int>("DocumentId");

                    b.HasKey("DistributionGroupId", "DocumentId");

                    b.HasIndex("DocumentId");

                    b.ToTable("DistributionDocuments");
                });

            modelBuilder.Entity("Web.Models.DistributionGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedByUserId")
                        .IsRequired()
                        .HasMaxLength(450);

                    b.Property<DateTimeOffset>("CreatedOn");

                    b.Property<DateTimeOffset>("ModifiedOn");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(56);

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.ToTable("DistributionGroups");
                });

            modelBuilder.Entity("Web.Models.DistributionRecipient", b =>
                {
                    b.Property<int>("DistributionGroupId");

                    b.Property<string>("ApplicationUserId");

                    b.Property<int>("Permissions");

                    b.HasKey("DistributionGroupId", "ApplicationUserId");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("DistributionRecipients");
                });

            modelBuilder.Entity("Web.Models.Document", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Abstract")
                        .HasMaxLength(512);

                    b.Property<string>("CreatedByUserId")
                        .IsRequired()
                        .HasMaxLength(450);

                    b.Property<DateTimeOffset>("CreatedOn");

                    b.Property<DateTimeOffset>("ModifiedOn");

                    b.Property<int>("Status");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(60);

                    b.HasKey("Id");

                    b.ToTable("Documents");
                });

            modelBuilder.Entity("Web.Models.DocumentContent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<int>("DocumentId");

                    b.HasKey("Id");

                    b.HasIndex("DocumentId")
                        .IsUnique();

                    b.ToTable("DocumentContents");
                });

            modelBuilder.Entity("Web.Models.File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedByUserId")
                        .IsRequired()
                        .HasMaxLength(450);

                    b.Property<DateTimeOffset>("CreatedOn");

                    b.Property<int>("DocumentId");

                    b.Property<string>("Extension")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.Property<string>("Key")
                        .HasMaxLength(1024);

                    b.Property<DateTimeOffset>("ModifiedOn");

                    b.Property<int>("PageCount");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<long>("Size");

                    b.Property<int>("Status");

                    b.Property<string>("ThumbnailPath")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<int>("VersionNum");

                    b.HasKey("Id");

                    b.HasIndex("DocumentId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("Web.Models.PermissionType", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(56);

                    b.HasKey("Id");

                    b.ToTable("PermissionTypes","Lookup");
                });

            modelBuilder.Entity("Web.Models.StatusType", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(56);

                    b.HasKey("Id");

                    b.ToTable("StatusTypes","Lookup");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Web.Models.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Web.Models.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Web.Models.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Web.Models.DistributionDocument", b =>
                {
                    b.HasOne("Web.Models.DistributionGroup", "DistributionGroup")
                        .WithMany("DistributionDocuments")
                        .HasForeignKey("DistributionGroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Web.Models.Document", "Document")
                        .WithMany("Distributions")
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Web.Models.DistributionRecipient", b =>
                {
                    b.HasOne("Web.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("LibraryAccessList")
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Web.Models.DistributionGroup", "DistributionGroup")
                        .WithMany("Recipients")
                        .HasForeignKey("DistributionGroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Web.Models.DocumentContent", b =>
                {
                    b.HasOne("Web.Models.Document", "Document")
                        .WithOne("Content")
                        .HasForeignKey("Web.Models.DocumentContent", "DocumentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Web.Models.File", b =>
                {
                    b.HasOne("Web.Models.Document", "Document")
                        .WithMany("Files")
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
