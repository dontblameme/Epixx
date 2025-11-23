using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Epixx.Migrations
{
    /// <inheritdoc />
    public partial class ForeignKeyhustle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pallets_Drivers_DriverId",
                table: "Pallets");

            migrationBuilder.DropForeignKey(
                name: "FK_PalletSpots_Pallets_CurrentPalletId",
                table: "PalletSpots");

            migrationBuilder.DropIndex(
                name: "IX_PalletSpots_CurrentPalletId",
                table: "PalletSpots");

            migrationBuilder.CreateIndex(
                name: "IX_PalletSpots_CurrentPalletId",
                table: "PalletSpots",
                column: "CurrentPalletId",
                unique: true,
                filter: "[CurrentPalletId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Pallets_Drivers_DriverId",
                table: "Pallets",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PalletSpots_Pallets_CurrentPalletId",
                table: "PalletSpots",
                column: "CurrentPalletId",
                principalTable: "Pallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pallets_Drivers_DriverId",
                table: "Pallets");

            migrationBuilder.DropForeignKey(
                name: "FK_PalletSpots_Pallets_CurrentPalletId",
                table: "PalletSpots");

            migrationBuilder.DropIndex(
                name: "IX_PalletSpots_CurrentPalletId",
                table: "PalletSpots");

            migrationBuilder.CreateIndex(
                name: "IX_PalletSpots_CurrentPalletId",
                table: "PalletSpots",
                column: "CurrentPalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pallets_Drivers_DriverId",
                table: "Pallets",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PalletSpots_Pallets_CurrentPalletId",
                table: "PalletSpots",
                column: "CurrentPalletId",
                principalTable: "Pallets",
                principalColumn: "Id");
        }
    }
}
