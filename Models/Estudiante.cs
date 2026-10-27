using System.ComponentModel.DataAnnotations;

namespace DatosPersonalesApi.Models;

public class Estudiante
{
    public int Id { get; set; }

    [Display(Name = "matrícula")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(20, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    public string Matricula { get; set; } = string.Empty;

    [Display(Name = "nombre")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(120, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "correo electrónico")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [EmailAddress(ErrorMessage = "El campo {0} debe contener una dirección de correo válida.")]
    [StringLength(254, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    public string Correo { get; set; } = string.Empty;
}
