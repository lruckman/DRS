using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Data.Migrations
{
    public partial class distributiongrouptype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DistributionDocuments");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "DistributionGroups",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Distributions",
                columns: table => new
                {
                    DistributionGroupId = table.Column<int>(nullable: false),
                    DocumentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distributions", x => new { x.DistributionGroupId, x.DocumentId });
                    table.ForeignKey(
                        name: "FK_Distributions_DistributionGroups_DistributionGroupId",
                        column: x => x.DistributionGroupId,
                        principalTable: "DistributionGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Distributions_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DistributionGroupTypes",
                schema: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 56, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistributionGroupTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Distributions_DocumentId",
                table: "Distributions",
                column: "DocumentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Distributions");

            migrationBuilder.DropTable(
                name: "DistributionGroupTypes",
                schema: "Lookup");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "DistributionGroups");

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

            migrationBuilder.CreateIndex(
                name: "IX_DistributionDocuments_DocumentId",
                table: "DistributionDocuments",
                column: "DocumentId");
        }
    }
}
