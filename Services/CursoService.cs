using DatosPersonalesApi.Models;

namespace DatosPersonalesApi.Services;

public sealed class CursoService
{
    private readonly List<Curso> _cursos =
    [
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
        }
    ];

    private readonly object _lock = new();

    public IReadOnlyList<Curso> GetAll()
    {
        lock (_lock)
        {
            return _cursos.Select(Copy).ToArray();
        }
    }

    public Curso? GetById(int id)
    {
        lock (_lock)
        {
            var curso = _cursos.Find(curso => curso.Id == id);
            return curso is null ? null : Copy(curso);
        }
    }

    public Curso Create(Curso curso)
    {
        lock (_lock)
        {
            var nuevoCurso = Copy(curso);
            nuevoCurso.Id = _cursos.Count == 0 ? 1 : _cursos.Max(curso => curso.Id) + 1;
            _cursos.Add(nuevoCurso);
            return Copy(nuevoCurso);
        }
    }

    public Curso? Update(int id, Curso curso)
    {
        lock (_lock)
        {
            var existingCourse = _cursos.Find(existingCourse => existingCourse.Id == id);
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

            return Copy(existingCourse);
        }
    }

    public bool Delete(int id)
    {
        lock (_lock)
        {
            var curso = _cursos.Find(curso => curso.Id == id);
            return curso is not null && _cursos.Remove(curso);
        }
    }

    private static Curso Copy(Curso curso) => new()
    {
        Id = curso.Id,
        Codigo = curso.Codigo,
        Nombre = curso.Nombre,
        Area = curso.Area,
        Horas = curso.Horas,
        CupoMaximo = curso.CupoMaximo,
        FechaInicio = curso.FechaInicio
    };
}
