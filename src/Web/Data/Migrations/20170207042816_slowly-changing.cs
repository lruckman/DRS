using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Web.data.migrations
{
    public partial class slowlychanging : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_DocumentId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Abstract",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "Key",
                table: "Files",
                newName: "AccessKey");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Files",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Documents",
                newName: "CreatedBy");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EndDate",
                table: "Files",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                columns: new[] { "DocumentId", "VersionNum" });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Metadata");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Files",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "AccessKey",
                table: "Files",
                newName: "Key");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Documents",
                newName: "CreatedByUserId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Files",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModifiedOn",
                table: "Files",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "Abstract",
                table: "Documents",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModifiedOn",
                table: "Documents",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Documents",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Documents",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Files_DocumentId",
                table: "Files",
                column: "DocumentId");
        }
    }
}
