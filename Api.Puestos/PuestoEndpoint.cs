using Api.Puestos.Entities;
using Api.Puestos.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Puestos
{
    public static class PuestoEndpoints
    {
        public static void MapPuestoEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes
                .MapGroup("/api/puestos")
                .WithTags(nameof(Puesto))
                .RequireCors("ReactDev");

            group.MapGet("/activos", async ([FromServices] IPuestoService puestoService) =>
            {
                var puestos = await puestoService.GetActivosAsync();
                return Results.Ok(puestos);
            })
            .WithName("GetPuestosActivos")
            .WithOpenApi();
        }
    }
}