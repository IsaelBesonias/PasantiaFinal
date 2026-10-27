using DatosPersonalesApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DatosPersonalesApi.Data;

public sealed class CursoDbContext(DbContextOptions<CursoDbContext> options) : DbContext(options)
{
    public DbSet<Curso> Cursos => Set<Curso>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Curso>().HasData(
            new Curso
            {
                Id = 1,
                Codigo = "INF101",
                Nombre = "Introducción a la programación",
                Area = "Informática",
                Horas = 48,
                CupoMaximo = 30,
                FechaInicio = new DateOnly(2026, 9, 7)
            },
            new Curso
            {
                Id = 2,
                Codigo = "ADM201",
                Nombre = "Administración de empresas",
                Area = "Negocios",
                Horas = 36,
                CupoMaximo = 25,
                FechaInicio = new DateOnly(2026, 9, 14)
            },
            new Curso
            {
                Id = 3,
                Codigo = "MAT105",
                Nombre = "Matemática básica",
                Area = "Ciencias",
                Horas = 40,
                CupoMaximo = 35,
                FechaInicio = new DateOnly(2026, 9, 21)
            });
    }
}
