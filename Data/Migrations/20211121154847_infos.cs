using Microsoft.EntityFrameworkCore.Migrations;

namespace dm.Shibalana.Data.Migrations
{
    public partial class infos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AddressInfos",
                columns: table => new
                {
                    AddressInfoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPartOfTeam = table.Column<bool>(type: "bit", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(20,9)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressInfos", x => x.AddressInfoId);
                });

            migrationBuilder.InsertData(
                table: "AddressInfos",
                columns: new[] { "AddressInfoId", "Address", "Amount", "IsPartOfTeam", "Label" },
                values: new object[,]
                {
                    { 1, "2vQBVYD6fn1Z4iA2JWo3qY1tMBanspkHY4TfLe51hj9b", 0m, true, "Team" },
                    { 2, "Ewg558ARXCoEtCHeGmicUznRWvbae6eczGUFS1tkPBX8", 0m, true, "Staking" },
                    { 3, "A8GFDkvqg6WLGTsXksiabD3oKeB2aQXtNoG98R9Hr5QG", 0m, true, "Marketing" },
                    { 4, "9huAyo2PytpiqqNDvvd5tb2nQNXpjE6xKS81M4BdeVES", 0m, true, "Liquidity" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddressInfos_IsPartOfTeam",
                table: "AddressInfos",
                column: "IsPartOfTeam");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddressInfos");
        }
    }
}
