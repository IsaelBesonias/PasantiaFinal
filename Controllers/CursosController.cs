using DatosPersonalesApi.Models;
using DatosPersonalesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DatosPersonalesApi.Controllers;

[ApiController]
[Route("api/cursos")]
public class CursosController(CursoService cursoService, InscripcionService inscripcionService) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<Curso>> GetAll()
    {
        return Ok(cursoService.GetAll());
    }

    [HttpGet("{id:int}")]
    public ActionResult<Curso> GetById(int id)
    {
        var curso = cursoService.GetById(id);
        return curso is null ? CourseNotFound() : Ok(curso);
    }

    [HttpPost]
    public ActionResult<Curso> Create(Curso curso)
    {
        var nuevoCurso = cursoService.Create(curso);
        return CreatedAtAction(nameof(GetById), new { id = nuevoCurso.Id }, nuevoCurso);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Curso> Update(int id, Curso curso)
    {
        var cursoActualizado = cursoService.Update(id, curso);
        return cursoActualizado is null ? CourseNotFound() : Ok(cursoActualizado);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return cursoService.Delete(id) ? NoContent() : CourseNotFound();
    }

    [HttpGet("{id:int}/estudiantes")]
    public ActionResult<IReadOnlyList<EstudianteInscritoResponse>> GetStudents(int id)
    {
        var estudiantes = inscripcionService.GetEstudiantesDeCurso(id);
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
