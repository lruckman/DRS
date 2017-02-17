using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Web.data.migrations
{
    public partial class tphactionrenames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Metadata");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.CreateTable(
                name: "Revisions",
                columns: table => new
                {
                    DocumentId = table.Column<int>(nullable: false),
                    VersionNum = table.Column<int>(nullable: false),
                    Abstract = table.Column<string>(maxLength: 512, nullable: true),
                    AccessKey = table.Column<string>(maxLength: 1024, nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    EndDate = table.Column<DateTimeOffset>(nullable: true),
                    Extension = table.Column<string>(maxLength: 16, nullable: false),
                    IndexDate = table.Column<DateTimeOffset>(nullable: true),
                    PageCount = table.Column<int>(nullable: false),
                    Path = table.Column<string>(maxLength: 256, nullable: false),
                    Size = table.Column<long>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    ThumbnailPath = table.Column<string>(maxLength: 256, nullable: false),
                    Title = table.Column<string>(maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Revisions", x => new { x.DocumentId, x.VersionNum });
                    table.ForeignKey(
                        name: "FK_Revisions_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Revisions");

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    DocumentId = table.Column<int>(nullable: false),
                    VersionNum = table.Column<int>(nullable: false),
                    AccessKey = table.Column<string>(maxLength: 1024, nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    EndDate = table.Column<DateTimeOffset>(nullable: true),
                    Extension = table.Column<string>(maxLength: 16, nullable: false),
                    PageCount = table.Column<int>(nullable: false),
                    Path = table.Column<string>(maxLength: 256, nullable: false),
                    Size = table.Column<long>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    ThumbnailPath = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => new { x.DocumentId, x.VersionNum });
                    table.ForeignKey(
                        name: "FK_Files_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Metadata",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Abstract = table.Column<string>(maxLength: 512, nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    DocumentId = table.Column<int>(nullable: false),
                    EndDate = table.Column<DateTimeOffset>(nullable: true),
                    Title = table.Column<string>(maxLength: 60, nullable: false),
                    VersionNum = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metadata", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Metadata_Files_DocumentId_VersionNum",
                        columns: x => new { x.DocumentId, x.VersionNum },
                        principalTable: "Files",
                        principalColumns: new[] { "DocumentId", "VersionNum" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Metadata_DocumentId_VersionNum",
                table: "Metadata",
                columns: new[] { "DocumentId", "VersionNum" });
        }
    }
}
