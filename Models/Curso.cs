using System.ComponentModel.DataAnnotations;

namespace DatosPersonalesApi.Models;

public class Curso
{
    public int Id { get; set; }

    [Display(Name = "código")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(30, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    public string Codigo { get; set; } = string.Empty;

    [Display(Name = "nombre")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(120, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "área")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(80, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    public string Area { get; set; } = string.Empty;

    [Display(Name = "horas")]
    [Range(1, int.MaxValue, ErrorMessage = "El campo {0} debe ser como mínimo {1}.")]
    public int Horas { get; set; }

    [Display(Name = "cupo máximo")]
    [Range(1, int.MaxValue, ErrorMessage = "El campo {0} debe ser como mínimo {1}.")]
    public int CupoMaximo { get; set; }

    [Display(Name = "fecha de inicio")]
    [Range(typeof(DateOnly), "0001-01-02", "9999-12-31",
        ErrorMessage = "El campo {0} debe ser posterior al 1 de enero del año 0001.")]
    public DateOnly FechaInicio { get; set; }
}
