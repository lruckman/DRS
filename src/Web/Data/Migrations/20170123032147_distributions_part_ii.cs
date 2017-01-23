using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Web.Data.Migrations
{
    public partial class distributions_part_ii : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentDistributions");

            migrationBuilder.DropTable(
                name: "LibraryDocuments");

            migrationBuilder.DropTable(
                name: "UserLibraries");

            migrationBuilder.DropTable(
                name: "Libraries");

            migrationBuilder.CreateTable(
                name: "DistributionDocuments",
                columns: table => new
                {
                    DistributionGroupId = table.Column<int>(nullable: false),
                    DocumentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistributionDocuments", x => new { x.DistributionGroupId, x.DocumentId });
                    table.ForeignKey(
                        name: "FK_DistributionDocuments_DistributionGroups_DistributionGroupId",
                        column: x => x.DistributionGroupId,
                        principalTable: "DistributionGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DistributionDocuments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DistributionRecipients",
                columns: table => new
                {
                    DistributionGroupId = table.Column<int>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: false),
                    Permissions = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistributionRecipients", x => new { x.DistributionGroupId, x.ApplicationUserId });
                    table.ForeignKey(
                        name: "FK_DistributionRecipients_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DistributionRecipients_DistributionGroups_DistributionGroupId",
                        column: x => x.DistributionGroupId,
                        principalTable: "DistributionGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DistributionDocuments_DocumentId",
                table: "DistributionDocuments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DistributionRecipients_ApplicationUserId",
                table: "DistributionRecipients",
                column: "ApplicationUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DistributionDocuments");

            migrationBuilder.DropTable(
                name: "DistributionRecipients");

            migrationBuilder.CreateTable(
                name: "DocumentDistributions",
                columns: table => new
                {
                    DistributionGroupId = table.Column<int>(nullable: false),
                    DocumentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentDistributions", x => new { x.DistributionGroupId, x.DocumentId });
                    table.ForeignKey(
                        name: "FK_DocumentDistributions_DistributionGroups_DistributionGroupId",
                        column: x => x.DistributionGroupId,
                        principalTable: "DistributionGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentDistributions_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Libraries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedByUserId = table.Column<string>(maxLength: 450, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: false),
                    Name = table.Column<string>(maxLength: 56, nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Libraries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LibraryDocuments",
                columns: table => new
                {
                    LibraryId = table.Column<int>(nullable: false),
                    DocumentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LibraryDocuments", x => new { x.LibraryId, x.DocumentId });
                    table.ForeignKey(
                        name: "FK_LibraryDocuments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LibraryDocuments_Libraries_LibraryId",
                        column: x => x.LibraryId,
                        principalTable: "Libraries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLibraries",
                columns: table => new
                {
                    LibraryId = table.Column<int>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: false),
                    Permissions = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLibraries", x => new { x.LibraryId, x.ApplicationUserId });
                    table.ForeignKey(
                        name: "FK_UserLibraries_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLibraries_Libraries_LibraryId",
                        column: x => x.LibraryId,
                        principalTable: "Libraries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDistributions_DocumentId",
                table: "DocumentDistributions",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryDocuments_DocumentId",
                table: "LibraryDocuments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLibraries_ApplicationUserId",
                table: "UserLibraries",
                column: "ApplicationUserId");
        }
    }
}
