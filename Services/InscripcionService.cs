using DatosPersonalesApi.Models;

namespace DatosPersonalesApi.Services;

public sealed class InscripcionService(CursoService cursoService)
{
    private readonly List<Estudiante> _estudiantes =
    [
        new Estudiante
        {
            Id = 1,
            Matricula = "2024-0001",
            Nombre = "María Rodríguez",
            Correo = "maria.rodriguez@institucion.edu"
        },
        new Estudiante
        {
            Id = 2,
            Matricula = "2024-0002",
            Nombre = "Juan Pérez",
            Correo = "juan.perez@institucion.edu"
        },
        new Estudiante
        {
            Id = 3,
            Matricula = "2024-0003",
            Nombre = "Laura Gómez",
            Correo = "laura.gomez@institucion.edu"
        }
    ];

    private readonly List<Inscripcion> _inscripciones = [];
    private readonly object _lock = new();

    public IReadOnlyList<Estudiante> GetEstudiantes()
    {
        lock (_lock)
        {
            return _estudiantes.Select(CopiarEstudiante).ToArray();
        }
    }

    public Estudiante CreateEstudiante(Estudiante estudiante)
    {
        lock (_lock)
        {
            var nuevoEstudiante = CopiarEstudiante(estudiante);
            nuevoEstudiante.Id = _estudiantes.Count == 0 ? 1 : _estudiantes.Max(item => item.Id) + 1;
            _estudiantes.Add(nuevoEstudiante);
            return CopiarEstudiante(nuevoEstudiante);
        }
    }

    public IReadOnlyList<EstudianteInscritoResponse>? GetEstudiantesDeCurso(int cursoId)
    {
        lock (_lock)
        {
            if (cursoService.GetById(cursoId) is null)
            {
                return null;
            }

            return _inscripciones
                .Where(inscripcion => inscripcion.CursoId == cursoId)
                .Select(inscripcion =>
                {
                    var estudiante = _estudiantes.Single(estudiante => estudiante.Id == inscripcion.EstudianteId);
                    return new EstudianteInscritoResponse(
                        inscripcion.Id,
                        estudiante.Id,
                        estudiante.Matricula,
                        estudiante.Nombre,
                        estudiante.Correo,
                        inscripcion.Fecha);
                })
                .ToArray();
        }
    }

    public ResultadoInscripcion CreateInscripcion(CrearInscripcionRequest request)
    {
        lock (_lock)
        {
            var curso = cursoService.GetById(request.CursoId);
            if (curso is null)
            {
                return new ResultadoInscripcion(null, "No existe un curso con el identificador indicado.", true);
            }

            if (_estudiantes.All(estudiante => estudiante.Id != request.EstudianteId))
            {
                return new ResultadoInscripcion(null, "No existe un estudiante con el identificador indicado.", true);
            }

            if (_inscripciones.Any(inscripcion =>
                    inscripcion.EstudianteId == request.EstudianteId
                    && inscripcion.CursoId == request.CursoId))
            {
                return new ResultadoInscripcion(
                    null,
                    "El estudiante ya está inscrito en este curso.",
                    false);
            }

            var inscritosEnCurso = _inscripciones.Count(inscripcion => inscripcion.CursoId == request.CursoId);
            if (inscritosEnCurso >= curso.CupoMaximo)
            {
                return new ResultadoInscripcion(
                    null,
                    "El curso ya alcanzó el cupo máximo. No hay cupos disponibles.",
                    false);
            }

            var inscripcion = new Inscripcion
            {
                Id = _inscripciones.Count == 0 ? 1 : _inscripciones.Max(item => item.Id) + 1,
                EstudianteId = request.EstudianteId,
                CursoId = request.CursoId,
                Fecha = DateTimeOffset.UtcNow
            };

            _inscripciones.Add(inscripcion);
            return new ResultadoInscripcion(CopiarInscripcion(inscripcion), null, false);
        }
    }

    private static Estudiante CopiarEstudiante(Estudiante estudiante) => new()
    {
        Id = estudiante.Id,
        Matricula = estudiante.Matricula,
        Nombre = estudiante.Nombre,
        Correo = estudiante.Correo
    };

    private static Inscripcion CopiarInscripcion(Inscripcion inscripcion) => new()
    {
        Id = inscripcion.Id,
        EstudianteId = inscripcion.EstudianteId,
        CursoId = inscripcion.CursoId,
        Fecha = inscripcion.Fecha
    };
}
