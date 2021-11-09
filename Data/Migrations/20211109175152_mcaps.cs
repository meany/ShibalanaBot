using Microsoft.EntityFrameworkCore.Migrations;

namespace dm.Shibalana.Data.Migrations
{
    public partial class mcaps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceBTC",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "PriceBTC100k",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "PriceBTCChange",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "PriceBTCChangePct",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "PriceETH",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "PriceETH100k",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "PriceETHChange",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "PriceETHChangePct",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "PriceUSD100k",
                table: "Prices");

            migrationBuilder.AlterColumn<decimal>(
                name: "FullMarketCapUSD",
                table: "Prices",
                type: "decimal(16,8)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "FullMarketCapUSD",
                table: "Prices",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(16,8)");

            migrationBuilder.AddColumn<decimal>(
                name: "PriceBTC",
                table: "Prices",
                type: "decimal(16,8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceBTC100k",
                table: "Prices",
                type: "decimal(16,8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PriceBTCChange",
                table: "Prices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceBTCChangePct",
                table: "Prices",
                type: "decimal(12,8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceETH",
                table: "Prices",
                type: "decimal(16,8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceETH100k",
                table: "Prices",
                type: "decimal(16,8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PriceETHChange",
                table: "Prices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceETHChangePct",
                table: "Prices",
                type: "decimal(12,8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceUSD100k",
                table: "Prices",
                type: "decimal(11,6)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
