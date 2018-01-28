using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Web.data.migrations
{
    public partial class datafiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "AccessKey",
                table: "Revisions");

            migrationBuilder.DropColumn(
                name: "Extension",
                table: "Revisions");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "Revisions");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Revisions");

            migrationBuilder.DropColumn(
                name: "ThumbnailPath",
                table: "Revisions");

            migrationBuilder.RenameColumn(
                name: "PageCount",
                table: "Revisions",
                newName: "DataFileId");

            migrationBuilder.CreateTable(
                name: "DataFiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    Extension = table.Column<string>(maxLength: 16, nullable: false),
                    IV = table.Column<string>(maxLength: 1024, nullable: true),
                    Key = table.Column<string>(maxLength: 1024, nullable: true),
                    PageCount = table.Column<int>(nullable: false),
                    Path = table.Column<string>(maxLength: 256, nullable: false),
                    Size = table.Column<long>(nullable: false),
                    ThumbnailPath = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataFiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Revisions_DataFileId",
                table: "Revisions",
                column: "DataFileId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Revisions_DataFiles_DataFileId",
                table: "Revisions",
                column: "DataFileId",
                principalTable: "DataFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Revisions_DataFiles_DataFileId",
                table: "Revisions");

            migrationBuilder.DropTable(
                name: "DataFiles");

            migrationBuilder.DropIndex(
                name: "IX_Revisions_DataFileId",
                table: "Revisions");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.RenameColumn(
                name: "DataFileId",
                table: "Revisions",
                newName: "PageCount");

            migrationBuilder.AddColumn<string>(
                name: "AccessKey",
                table: "Revisions",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Extension",
                table: "Revisions",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Revisions",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "Size",
                table: "Revisions",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailPath",
                table: "Revisions",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);
        }
    }
}
