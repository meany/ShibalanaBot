using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace dm.Shibalana.Data.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    PriceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    Group = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriceSHIBAForOneUSDC = table.Column<decimal>(type: "decimal(16,8)", nullable: false),
                    PriceUSDCForOneSHIBA = table.Column<decimal>(type: "decimal(16,8)", nullable: false),
                    PriceUSD = table.Column<decimal>(type: "decimal(11,6)", nullable: false),
                    PriceUSD100k = table.Column<decimal>(type: "decimal(11,6)", nullable: false),
                    PriceUSDChange = table.Column<int>(type: "int", nullable: false),
                    PriceUSDChangePct = table.Column<decimal>(type: "decimal(12,8)", nullable: false),
                    PriceETH = table.Column<decimal>(type: "decimal(16,8)", nullable: false),
                    PriceETH100k = table.Column<decimal>(type: "decimal(16,8)", nullable: false),
                    PriceETHChange = table.Column<int>(type: "int", nullable: false),
                    PriceETHChangePct = table.Column<decimal>(type: "decimal(12,8)", nullable: false),
                    PriceBTC = table.Column<decimal>(type: "decimal(16,8)", nullable: false),
                    PriceBTC100k = table.Column<decimal>(type: "decimal(16,8)", nullable: false),
                    PriceBTCChange = table.Column<int>(type: "int", nullable: false),
                    PriceBTCChangePct = table.Column<decimal>(type: "decimal(12,8)", nullable: false),
                    FullMarketCapUSD = table.Column<int>(type: "int", nullable: false),
                    CircMarketCapUSD = table.Column<int>(type: "int", nullable: false),
                    MarketCapUSDChange = table.Column<int>(type: "int", nullable: false),
                    MarketCapUSDChangePct = table.Column<decimal>(type: "decimal(12,8)", nullable: false),
                    VolumeUSD = table.Column<int>(type: "int", nullable: false),
                    VolumeUSDChange = table.Column<int>(type: "int", nullable: false),
                    VolumeUSDChangePct = table.Column<decimal>(type: "decimal(12,8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.PriceId);
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Response = table.Column<int>(type: "int", nullable: false),
                    DiscordUserId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    DiscordUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRateLimited = table.Column<bool>(type: "bit", nullable: false),
                    Command = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.RequestId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prices_Date",
                table: "Prices",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_Group",
                table: "Prices",
                column: "Group");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_Date",
                table: "Requests",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_Response_Command",
                table: "Requests",
                columns: new[] { "Response", "Command" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "Requests");
        }
    }
}
