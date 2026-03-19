using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Epixx.Migrations
{
    /// <inheritdoc />
    public partial class uupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pallets_Stores_LoadingDockId",
                table: "Pallets");

            migrationBuilder.DropIndex(
                name: "IX_Pallets_LoadingDockId",
                table: "Pallets");

            migrationBuilder.DropColumn(
                name: "LoadingDockId",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pallets_Stores_StoreId",
                table: "Pallets");

            migrationBuilder.DropIndex(
                name: "IX_Pallets_StoreId",
                table: "Pallets");

            migrationBuilder.AddColumn<int>(
                name: "LoadingDockId",
                table: "Pallets",
                type: "int",
                nullable: true);

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
        }
    }
}
