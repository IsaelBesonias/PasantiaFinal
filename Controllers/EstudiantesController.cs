using DatosPersonalesApi.Models;
using DatosPersonalesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DatosPersonalesApi.Controllers;

[ApiController]
[Route("api/estudiantes")]
public class EstudiantesController(InscripcionService inscripcionService) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<Estudiante>> GetAll()
    {
        return Ok(inscripcionService.GetEstudiantes());
    }

    [HttpPost]
    public ActionResult<Estudiante> Create(Estudiante estudiante)
    {
        var nuevoEstudiante = inscripcionService.CreateEstudiante(estudiante);
        return CreatedAtAction(nameof(GetAll), nuevoEstudiante);
    }
}
