using DatosPersonalesApi.Models;
using DatosPersonalesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DatosPersonalesApi.Controllers;

[ApiController]
[Route("api/cursos")]
public class CursosController(CursoService cursoService) : ControllerBase
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
        return curso is null ? NotFound() : Ok(curso);
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
        return cursoActualizado is null ? NotFound() : Ok(cursoActualizado);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return cursoService.Delete(id) ? NoContent() : NotFound();
    }
}
