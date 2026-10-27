using System.ComponentModel.DataAnnotations;

namespace DatosPersonalesApi.Models;

/// <summary>Datos necesarios para inscribir a un estudiante en un curso.</summary>
public sealed record CrearInscripcionRequest
{
    [Display(Name = "identificador del estudiante")]
    [Range(1, int.MaxValue, ErrorMessage = "El campo {0} debe ser como mínimo {1}.")]
    public int EstudianteId { get; init; }

    [Display(Name = "identificador del curso")]
    [Range(1, int.MaxValue, ErrorMessage = "El campo {0} debe ser como mínimo {1}.")]
    public int CursoId { get; init; }
}

/// <summary>Inscripción realizada junto con los datos básicos del estudiante.</summary>
public sealed record EstudianteInscritoResponse(
    int InscripcionId,
    int EstudianteId,
    string Matricula,
    string Nombre,
    string Correo,
    DateTimeOffset Fecha);

/// <summary>Resultado de intentar crear una inscripción.</summary>
public sealed record ResultadoInscripcion(
    Inscripcion? Inscripcion,
    string? Error,
    bool RecursoNoEncontrado);
