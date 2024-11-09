using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolicitudesService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SolicitudesVacaciones",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "EstaAprobada",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "SolicitudesPersonales");

            migrationBuilder.DropColumn(
                name: "EstaAprobada",
                table: "SolicitudesPersonales");

            migrationBuilder.DropColumn(
                name: "EstaAprobada",
                table: "SolicitudesHorasExtra");

            migrationBuilder.DropColumn(
                name: "EstaAprobada",
                table: "SolicitudesDocumentos");

            migrationBuilder.RenameColumn(
                name: "FechaAprobacion",
                table: "SolicitudesVacaciones",
                newName: "FechaModificacion");

            migrationBuilder.RenameColumn(
                name: "IdSolicitudVacaciones",
                table: "SolicitudesVacaciones",
                newName: "CantidadDias");

            migrationBuilder.RenameColumn(
                name: "FechaAprobacion",
                table: "SolicitudesPersonales",
                newName: "FechaModificacion");

            migrationBuilder.RenameColumn(
                name: "IdSolicitudPersonal",
                table: "SolicitudesPersonales",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "FechaAprobacion",
                table: "SolicitudesHorasExtra",
                newName: "FechaModificacion");

            migrationBuilder.RenameColumn(
                name: "IdSolicitudHorasExtra",
                table: "SolicitudesHorasExtra",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "FechaAprobacion",
                table: "SolicitudesDocumentos",
                newName: "FechaModificacion");

            migrationBuilder.RenameColumn(
                name: "IdSolicitudDocumento",
                table: "SolicitudesDocumentos",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "CantidadDias",
                table: "SolicitudesVacaciones",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "SolicitudesVacaciones",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "CreadoPor",
                table: "SolicitudesVacaciones",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "SolicitudesVacaciones",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCambioEstado",
                table: "SolicitudesVacaciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "SolicitudesVacaciones",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModificadoPor",
                table: "SolicitudesVacaciones",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoRechazo",
                table: "SolicitudesVacaciones",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreadoPor",
                table: "SolicitudesPersonales",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "SolicitudesPersonales",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCambioEstado",
                table: "SolicitudesPersonales",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "SolicitudesPersonales",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModificadoPor",
                table: "SolicitudesPersonales",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Motivo",
                table: "SolicitudesPersonales",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MotivoRechazo",
                table: "SolicitudesPersonales",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreadoPor",
                table: "SolicitudesHorasExtra",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "SolicitudesHorasExtra",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCambioEstado",
                table: "SolicitudesHorasExtra",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "SolicitudesHorasExtra",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaTrabajo",
                table: "SolicitudesHorasExtra",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModificadoPor",
                table: "SolicitudesHorasExtra",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoRechazo",
                table: "SolicitudesHorasExtra",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "SolicitudesDocumentos",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "CreadoPor",
                table: "SolicitudesDocumentos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "SolicitudesDocumentos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCambioEstado",
                table: "SolicitudesDocumentos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "SolicitudesDocumentos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModificadoPor",
                table: "SolicitudesDocumentos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoRechazo",
                table: "SolicitudesDocumentos",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoDocumento",
                table: "SolicitudesDocumentos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SolicitudesVacaciones",
                table: "SolicitudesVacaciones",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SolicitudesVacaciones",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "CreadoPor",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "FechaCambioEstado",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "ModificadoPor",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "MotivoRechazo",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "CreadoPor",
                table: "SolicitudesPersonales");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "SolicitudesPersonales");

            migrationBuilder.DropColumn(
                name: "FechaCambioEstado",
                table: "SolicitudesPersonales");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "SolicitudesPersonales");

            migrationBuilder.DropColumn(
                name: "ModificadoPor",
                table: "SolicitudesPersonales");

            migrationBuilder.DropColumn(
                name: "Motivo",
                table: "SolicitudesPersonales");

            migrationBuilder.DropColumn(
                name: "MotivoRechazo",
                table: "SolicitudesPersonales");

            migrationBuilder.DropColumn(
                name: "CreadoPor",
                table: "SolicitudesHorasExtra");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "SolicitudesHorasExtra");

            migrationBuilder.DropColumn(
                name: "FechaCambioEstado",
                table: "SolicitudesHorasExtra");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "SolicitudesHorasExtra");

            migrationBuilder.DropColumn(
                name: "FechaTrabajo",
                table: "SolicitudesHorasExtra");

            migrationBuilder.DropColumn(
                name: "ModificadoPor",
                table: "SolicitudesHorasExtra");

            migrationBuilder.DropColumn(
                name: "MotivoRechazo",
                table: "SolicitudesHorasExtra");

            migrationBuilder.DropColumn(
                name: "CreadoPor",
                table: "SolicitudesDocumentos");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "SolicitudesDocumentos");

            migrationBuilder.DropColumn(
                name: "FechaCambioEstado",
                table: "SolicitudesDocumentos");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "SolicitudesDocumentos");

            migrationBuilder.DropColumn(
                name: "ModificadoPor",
                table: "SolicitudesDocumentos");

            migrationBuilder.DropColumn(
                name: "MotivoRechazo",
                table: "SolicitudesDocumentos");

            migrationBuilder.DropColumn(
                name: "TipoDocumento",
                table: "SolicitudesDocumentos");

            migrationBuilder.RenameColumn(
                name: "FechaModificacion",
                table: "SolicitudesVacaciones",
                newName: "FechaAprobacion");

            migrationBuilder.RenameColumn(
                name: "CantidadDias",
                table: "SolicitudesVacaciones",
                newName: "IdSolicitudVacaciones");

            migrationBuilder.RenameColumn(
                name: "FechaModificacion",
                table: "SolicitudesPersonales",
                newName: "FechaAprobacion");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "SolicitudesPersonales",
                newName: "IdSolicitudPersonal");

            migrationBuilder.RenameColumn(
                name: "FechaModificacion",
                table: "SolicitudesHorasExtra",
                newName: "FechaAprobacion");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "SolicitudesHorasExtra",
                newName: "IdSolicitudHorasExtra");

            migrationBuilder.RenameColumn(
                name: "FechaModificacion",
                table: "SolicitudesDocumentos",
                newName: "FechaAprobacion");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "SolicitudesDocumentos",
                newName: "IdSolicitudDocumento");

            migrationBuilder.AlterColumn<int>(
                name: "IdSolicitudVacaciones",
                table: "SolicitudesVacaciones",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<bool>(
                name: "EstaAprobada",
                table: "SolicitudesVacaciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "SolicitudesPersonales",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "EstaAprobada",
                table: "SolicitudesPersonales",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EstaAprobada",
                table: "SolicitudesHorasExtra",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "SolicitudesDocumentos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AddColumn<bool>(
                name: "EstaAprobada",
                table: "SolicitudesDocumentos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SolicitudesVacaciones",
                table: "SolicitudesVacaciones",
                column: "IdSolicitudVacaciones");
        }
    }
}
