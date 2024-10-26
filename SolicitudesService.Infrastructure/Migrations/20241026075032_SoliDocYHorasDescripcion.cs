using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolicitudesService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SoliDocYHorasDescripcion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Motivo",
                table: "SolicitudesPersonales",
                newName: "Descripcion");

            migrationBuilder.RenameColumn(
                name: "TipoDocumento",
                table: "SolicitudesDocumentos",
                newName: "Descripcion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "SolicitudesPersonales",
                newName: "Motivo");

            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "SolicitudesDocumentos",
                newName: "TipoDocumento");
        }
    }
}
