using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cometPOS.Migrations
{
    /// <inheritdoc />
    public partial class IndiceUnicoCodigoSucursal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Sucursales_Codigo",
                table: "Sucursales",
                column: "Codigo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sucursales_Codigo",
                table: "Sucursales");
        }
    }
}
