using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tesis.Migrations
{
    /// <inheritdoc />
    public partial class AgregadoAtributosEstudiantes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Apellido",
                table: "Estudiantes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DNI",
                table: "Estudiantes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "Estudiantes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Estudiantes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Legajo",
                table: "Estudiantes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NombreMadre",
                table: "Estudiantes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NombrePadre",
                table: "Estudiantes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NombreTutor",
                table: "Estudiantes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Numero",
                table: "Estudiantes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Estudiantes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefonoMadre",
                table: "Estudiantes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefonoPadre",
                table: "Estudiantes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefonoTutor",
                table: "Estudiantes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Apellido",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "DNI",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "Legajo",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "NombreMadre",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "NombrePadre",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "NombreTutor",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "Numero",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "TelefonoMadre",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "TelefonoPadre",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "TelefonoTutor",
                table: "Estudiantes");
        }
    }
}
