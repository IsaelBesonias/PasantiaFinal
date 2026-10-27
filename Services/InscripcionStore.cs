using DatosPersonalesApi.Models;

namespace DatosPersonalesApi.Services;

public sealed class InscripcionStore
{
    internal List<Estudiante> Estudiantes { get; } =
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

    internal List<Inscripcion> Inscripciones { get; } = [];
    internal object SyncRoot { get; } = new();
}
