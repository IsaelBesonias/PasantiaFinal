using DatosPersonalesApi.Data;
using DatosPersonalesApi.Services;
using DatosPersonalesApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(entry => entry.Value?.Errors.Count > 0)
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value!.Errors
                        .Select(error => error.ErrorMessage.StartsWith("El campo ", StringComparison.OrdinalIgnoreCase)
                            || error.ErrorMessage.StartsWith("La fecha de inicio ", StringComparison.OrdinalIgnoreCase)
                                ? error.ErrorMessage
                                : $"El valor ingresado para {GetSpanishFieldName(entry.Key)} no es válido.")
                        .ToArray());

            var problem = new ValidationProblemDetails(errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "La solicitud contiene errores de validación.",
                Detail = "Corrige los campos indicados e inténtalo nuevamente."
            };

            return new BadRequestObjectResult(problem);
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<CursoDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Cursos")));
builder.Services.AddScoped<CursoService>();
builder.Services.AddSingleton<InscripcionStore>();
builder.Services.AddScoped<InscripcionService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CursoDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();

static string GetSpanishFieldName(string fieldName)
{
    var propertyName = fieldName.Split('.').Last();
    if (string.IsNullOrWhiteSpace(propertyName) || propertyName == "$")
    {
        return "el cuerpo de la solicitud";
    }

    return propertyName.ToLowerInvariant() switch
    {
        "curso" => "el curso",
        "codigo" => "el código",
        "nombre" => "el nombre",
        "area" => "el área",
        "horas" => "las horas",
        "cupomaximo" => "el cupo máximo",
        "fechainicio" => "la fecha de inicio",
        "estudiante" => "el estudiante",
        "matricula" => "la matrícula",
        "correo" => "el correo electrónico",
        "estudianteid" => "el identificador del estudiante",
        "cursoid" => "el identificador del curso",
        _ => "el campo"
    };
}
