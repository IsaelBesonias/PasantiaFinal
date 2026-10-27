using System.ComponentModel.DataAnnotations;

namespace DatosPersonalesApi.Models;

public class Inscripcion
{
    public int Id { get; set; }

    [Display(Name = "identificador del estudiante")]
    [Range(1, int.MaxValue, ErrorMessage = "El campo {0} debe ser como mínimo {1}.")]
    public int EstudianteId { get; set; }

    [Display(Name = "identificador del curso")]
    [Range(1, int.MaxValue, ErrorMessage = "El campo {0} debe ser como mínimo {1}.")]
    public int CursoId { get; set; }

    public DateTimeOffset Fecha { get; set; }
}
