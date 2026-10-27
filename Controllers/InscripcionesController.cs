using DatosPersonalesApi.Models;
using DatosPersonalesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DatosPersonalesApi.Controllers;

[ApiController]
[Route("api/inscripciones")]
public class InscripcionesController(InscripcionService inscripcionService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Inscripcion>> Create(
        CrearInscripcionRequest request,
        CancellationToken cancellationToken)
    {
        var resultado = await inscripcionService.CreateInscripcionAsync(request, cancellationToken);
        if (resultado.Inscripcion is not null)
        {
            return CreatedAtAction(
                nameof(CursosController.GetStudents),
                "Cursos",
                new { id = resultado.Inscripcion.CursoId },
                resultado.Inscripcion);
        }

        var problem = new ProblemDetails
        {
            Title = resultado.RecursoNoEncontrado
                ? "No se pudo completar la inscripción."
                : "Inscripción no permitida.",
            Detail = resultado.Error,
            Status = resultado.RecursoNoEncontrado
                ? StatusCodes.Status404NotFound
                : StatusCodes.Status400BadRequest
        };

        return resultado.RecursoNoEncontrado ? NotFound(problem) : BadRequest(problem);
    }
}
