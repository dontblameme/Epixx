using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Epixx.Migrations
{
    /// <inheritdoc />
    public partial class CurrentPalletId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PalletSpots_Pallets_CurrentPalletId",
                table: "PalletSpots");

            migrationBuilder.DropIndex(
                name: "IX_PalletSpots_CurrentPalletId",
                table: "PalletSpots");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentPalletId",
                table: "PalletSpots",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentPalletId1",
                table: "PalletSpots",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PalletSpots_CurrentPalletId1",
                table: "PalletSpots",
                column: "CurrentPalletId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PalletSpots_Pallets_CurrentPalletId1",
                table: "PalletSpots",
                column: "CurrentPalletId1",
                principalTable: "Pallets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PalletSpots_Pallets_CurrentPalletId1",
                table: "PalletSpots");

            migrationBuilder.DropIndex(
                name: "IX_PalletSpots_CurrentPalletId1",
                table: "PalletSpots");

            migrationBuilder.DropColumn(
                name: "CurrentPalletId1",
                table: "PalletSpots");

            migrationBuilder.AlterColumn<int>(
                name: "CurrentPalletId",
                table: "PalletSpots",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PalletSpots_CurrentPalletId",
                table: "PalletSpots",
                column: "CurrentPalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_PalletSpots_Pallets_CurrentPalletId",
                table: "PalletSpots",
                column: "CurrentPalletId",
                principalTable: "Pallets",
                principalColumn: "Id");
        }
    }
}
