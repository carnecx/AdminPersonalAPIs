using Api.Oferentes.Entities;
using Api.Oferentes.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Api.Oferentes
{
    public static class OferenteEndpoints
    {
        public static void MapOferenteEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes
                .MapGroup("/api/oferentes")
                .WithTags(nameof(Oferente))
                .RequireCors("ReactDev");

            group.MapGet("/", async ([FromServices] IOferenteService oferenteService) =>
            {
                var oferentes = await oferenteService.GetAllAsync();
                return Results.Ok(oferentes);
            })
            .WithName("GetOferentes")
            .RequireAuthorization()
            .WithOpenApi();

            // Core8: detalle de un oferente por identificación
            group.MapGet("/{identificacion}", async (string identificacion, [FromServices] IOferenteService oferenteService) =>
            {
                var detalle = await oferenteService.GetDetalleByIdentificacionAsync(identificacion);
                return detalle is not null ? Results.Ok(detalle) : Results.NotFound();
            })
            .WithName("GetOferenteDetalle")
            .RequireAuthorization()
            .WithOpenApi();

            // Core2: oferentes relacionados a un puesto (vía concurso)
            group.MapGet("/por-puesto/{codigoPuesto}", async (string codigoPuesto, [FromServices] IOferenteService oferenteService) =>
            {
                var oferentes = await oferenteService.GetOferentesPorPuestoAsync(codigoPuesto);
                return Results.Ok(oferentes);
            })
            .WithName("GetOferentesPorPuesto")
            .RequireAuthorization()
            .WithOpenApi();

            // Aut3: postulación de un oferente a un puesto (con CV adjunto)
            group.MapPost("/postulacion", async (
                [FromForm] OferentePostulacion datos,
                [FromServices] IOferenteService oferenteService,
                IWebHostEnvironment env) =>
            {
                string? rutaCurriculum = null;

                if (datos.Curriculum is not null && datos.Curriculum.Length > 0)
                {
                    var carpetaCv = Path.Combine(env.ContentRootPath, "uploads", "curriculums");
                    Directory.CreateDirectory(carpetaCv);

                    var nombreArchivo = $"cv_{datos.Identificacion}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{Path.GetExtension(datos.Curriculum.FileName)}";
                    var rutaCompleta = Path.Combine(carpetaCv, nombreArchivo);

                    using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                    {
                        await datos.Curriculum.CopyToAsync(stream);
                    }

                    rutaCurriculum = $"uploads/curriculums/{nombreArchivo}";
                }

                var resultado = await oferenteService.CrearPostulacionAsync(datos, rutaCurriculum);

                if (resultado != "OK")
                    return Results.BadRequest(new { mensaje = resultado });

                return Results.Ok(new { mensaje = "Datos guardados de manera satisfactoria" });
            })
            .WithName("CrearPostulacionOferente")
            .DisableAntiforgery()
            .WithOpenApi();
        }
    }
}