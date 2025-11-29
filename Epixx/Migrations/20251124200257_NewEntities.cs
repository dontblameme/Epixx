using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Epixx.Migrations
{
    /// <inheritdoc />
    public partial class NewEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pallets_Stores_StoreId",
                table: "Pallets");

            migrationBuilder.DropIndex(
                name: "IX_Pallets_StoreId",
                table: "Pallets");

            migrationBuilder.AddColumn<int>(
                name: "LoadingDockId",
                table: "Stores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Weight",
                table: "PalletTypes",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "LoadingDockId",
                table: "Pallets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Weight",
                table: "Pallets",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "LoadingDocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadingDocks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stores_LoadingDockId",
                table: "Stores",
                column: "LoadingDockId");

            migrationBuilder.CreateIndex(
                name: "IX_Pallets_LoadingDockId",
                table: "Pallets",
                column: "LoadingDockId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pallets_Stores_LoadingDockId",
                table: "Pallets",
                column: "LoadingDockId",
                principalTable: "Stores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_LoadingDocks_LoadingDockId",
                table: "Stores",
                column: "LoadingDockId",
                principalTable: "LoadingDocks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pallets_Stores_LoadingDockId",
                table: "Pallets");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_LoadingDocks_LoadingDockId",
                table: "Stores");

            migrationBuilder.DropTable(
                name: "LoadingDocks");

            migrationBuilder.DropIndex(
                name: "IX_Stores_LoadingDockId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Pallets_LoadingDockId",
                table: "Pallets");

            migrationBuilder.DropColumn(
                name: "LoadingDockId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "PalletTypes");

            migrationBuilder.DropColumn(
                name: "LoadingDockId",
                table: "Pallets");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Pallets");

            migrationBuilder.CreateIndex(
                name: "IX_Pallets_StoreId",
                table: "Pallets",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pallets_Stores_StoreId",
                table: "Pallets",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id");
        }
    }
}
