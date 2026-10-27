using System.ComponentModel.DataAnnotations;

namespace DatosPersonalesApi.Models;

public class Curso
{
    public int Id { get; set; }

    [Required]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public string Area { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Horas { get; set; }

    [Range(1, int.MaxValue)]
    public int CupoMaximo { get; set; }

    public DateOnly FechaInicio { get; set; }
}
