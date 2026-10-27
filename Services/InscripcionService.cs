using DatosPersonalesApi.Models;

namespace DatosPersonalesApi.Services;

public sealed class InscripcionService(CursoService cursoService, InscripcionStore store)
{
    public IReadOnlyList<Estudiante> GetEstudiantes()
    {
        lock (store.SyncRoot)
        {
            return store.Estudiantes.Select(CopiarEstudiante).ToArray();
        }
    }

    public Estudiante CreateEstudiante(Estudiante estudiante)
    {
        lock (store.SyncRoot)
        {
            var nuevoEstudiante = CopiarEstudiante(estudiante);
            nuevoEstudiante.Id = store.Estudiantes.Count == 0 ? 1 : store.Estudiantes.Max(item => item.Id) + 1;
            store.Estudiantes.Add(nuevoEstudiante);
            return CopiarEstudiante(nuevoEstudiante);
        }
    }

    public async Task<IReadOnlyList<EstudianteInscritoResponse>?> GetEstudiantesDeCursoAsync(
        int cursoId,
        CancellationToken cancellationToken)
    {
        if (await cursoService.GetByIdAsync(cursoId, cancellationToken) is null)
        {
            return null;
        }

        lock (store.SyncRoot)
        {
            return store.Inscripciones
                .Where(inscripcion => inscripcion.CursoId == cursoId)
                .Select(inscripcion =>
                {
                    var estudiante = store.Estudiantes.Single(estudiante => estudiante.Id == inscripcion.EstudianteId);
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

    public async Task<ResultadoInscripcion> CreateInscripcionAsync(
        CrearInscripcionRequest request,
        CancellationToken cancellationToken)
    {
        var curso = await cursoService.GetByIdAsync(request.CursoId, cancellationToken);
        if (curso is null)
        {
            return new ResultadoInscripcion(null, "No existe un curso con el identificador indicado.", true);
        }

        lock (store.SyncRoot)
        {
            if (store.Estudiantes.All(estudiante => estudiante.Id != request.EstudianteId))
            {
                return new ResultadoInscripcion(null, "No existe un estudiante con el identificador indicado.", true);
            }

            if (store.Inscripciones.Any(inscripcion =>
                    inscripcion.EstudianteId == request.EstudianteId
                    && inscripcion.CursoId == request.CursoId))
            {
                return new ResultadoInscripcion(
                    null,
                    "El estudiante ya está inscrito en este curso.",
                    false);
            }

            var inscritosEnCurso = store.Inscripciones.Count(inscripcion => inscripcion.CursoId == request.CursoId);
            if (inscritosEnCurso >= curso.CupoMaximo)
            {
                return new ResultadoInscripcion(
                    null,
                    "El curso ya alcanzó el cupo máximo. No hay cupos disponibles.",
                    false);
            }

            var inscripcion = new Inscripcion
            {
                Id = store.Inscripciones.Count == 0 ? 1 : store.Inscripciones.Max(item => item.Id) + 1,
                EstudianteId = request.EstudianteId,
                CursoId = request.CursoId,
                Fecha = DateTimeOffset.UtcNow
            };

            store.Inscripciones.Add(inscripcion);
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
