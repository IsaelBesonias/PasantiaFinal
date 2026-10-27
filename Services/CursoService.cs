using DatosPersonalesApi.Data;
using DatosPersonalesApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DatosPersonalesApi.Services;

public sealed class CursoService(CursoDbContext dbContext)
{
    public async Task<IReadOnlyList<Curso>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Cursos
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<Curso?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return dbContext.Cursos
            .AsNoTracking()
            .FirstOrDefaultAsync(curso => curso.Id == id, cancellationToken);
    }

    public async Task<Curso> CreateAsync(Curso curso, CancellationToken cancellationToken)
    {
        dbContext.Cursos.Add(curso);
        await dbContext.SaveChangesAsync(cancellationToken);
        return curso;
    }

    public async Task<Curso?> UpdateAsync(int id, Curso curso, CancellationToken cancellationToken)
    {
        var existingCourse = await dbContext.Cursos
            .FirstOrDefaultAsync(existingCourse => existingCourse.Id == id, cancellationToken);
        if (existingCourse is null)
        {
            return null;
        }

        existingCourse.Codigo = curso.Codigo;
        existingCourse.Nombre = curso.Nombre;
        existingCourse.Area = curso.Area;
        existingCourse.Horas = curso.Horas;
        existingCourse.CupoMaximo = curso.CupoMaximo;
        existingCourse.FechaInicio = curso.FechaInicio;

        await dbContext.SaveChangesAsync(cancellationToken);
        return existingCourse;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var curso = await dbContext.Cursos
            .FirstOrDefaultAsync(curso => curso.Id == id, cancellationToken);
        if (curso is null)
        {
            return false;
        }

        dbContext.Cursos.Remove(curso);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
