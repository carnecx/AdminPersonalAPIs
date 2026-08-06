using Api.Puestos.Repository;
using Api.Puestos.Services;
using Api.Puestos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactDev", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<PuestoRepository>();
builder.Services.AddScoped<IPuestoService, PuestoService>();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("ReactDev");

app.MapPuestoEndpoints();
app.Run();