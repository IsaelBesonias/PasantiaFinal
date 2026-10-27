using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DatosPersonalesApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cursos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codigo = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Area = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Horas = table.Column<int>(type: "INTEGER", nullable: false),
                    CupoMaximo = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaInicio = table.Column<DateOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cursos", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Cursos",
                columns: new[] { "Id", "Area", "Codigo", "CupoMaximo", "FechaInicio", "Horas", "Nombre" },
                values: new object[,]
                {
                    { 1, "Informática", "INF101", 30, new DateOnly(2026, 9, 7), 48, "Introducción a la programación" },
                    { 2, "Negocios", "ADM201", 25, new DateOnly(2026, 9, 14), 36, "Administración de empresas" },
                    { 3, "Ciencias", "MAT105", 35, new DateOnly(2026, 9, 21), 40, "Matemática básica" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cursos");
        }
    }
}
