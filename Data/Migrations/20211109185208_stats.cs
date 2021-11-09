using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace dm.Shibalana.Data.Migrations
{
    public partial class stats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stats",
                columns: table => new
                {
                    StatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Group = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Supply = table.Column<decimal>(type: "decimal(20,9)", nullable: false),
                    Circulation = table.Column<decimal>(type: "decimal(20,9)", nullable: false),
                    Holders = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stats", x => x.StatId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stats_Date",
                table: "Stats",
                column: "Date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stats");
        }
    }
}
