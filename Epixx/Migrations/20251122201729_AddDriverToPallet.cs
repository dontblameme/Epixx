using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Epixx.Migrations
{
    /// <inheritdoc />
    public partial class AddDriverToPallet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "PalletTypes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Barcode",
                table: "PalletTypes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
