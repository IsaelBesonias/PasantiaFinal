using DatosPersonalesApi.Models;
using DatosPersonalesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DatosPersonalesApi.Controllers;

[ApiController]
[Route("api/cursos")]
public class CursosController(CursoService cursoService, InscripcionService inscripcionService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Curso>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await cursoService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Curso>> GetById(int id, CancellationToken cancellationToken)
    {
        var curso = await cursoService.GetByIdAsync(id, cancellationToken);
        return curso is null ? CourseNotFound() : Ok(curso);
    }

    [HttpPost]
    public async Task<ActionResult<Curso>> Create(Curso curso, CancellationToken cancellationToken)
    {
        var nuevoCurso = await cursoService.CreateAsync(curso, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = nuevoCurso.Id }, nuevoCurso);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Curso>> Update(int id, Curso curso, CancellationToken cancellationToken)
    {
        var cursoActualizado = await cursoService.UpdateAsync(id, curso, cancellationToken);
        return cursoActualizado is null ? CourseNotFound() : Ok(cursoActualizado);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await cursoService.DeleteAsync(id, cancellationToken) ? NoContent() : CourseNotFound();
    }

    [HttpGet("{id:int}/estudiantes")]
    public async Task<ActionResult<IReadOnlyList<EstudianteInscritoResponse>>> GetStudents(
        int id,
        CancellationToken cancellationToken)
    {
        var estudiantes = await inscripcionService.GetEstudiantesDeCursoAsync(id, cancellationToken);
        return estudiantes is null
            ? CourseNotFound()
            : Ok(estudiantes);
    }

    private static NotFoundObjectResult CourseNotFound() =>
        new(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Curso no encontrado.",
            Detail = "No existe un curso con el identificador indicado."
        });
}
