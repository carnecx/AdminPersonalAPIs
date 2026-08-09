var builder = WebApplication.CreateBuilder(args);

// configura cors para permitir solicitudes desde react
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactDev", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// carga las rutas del reverse proxy desde appsettings.json
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(
        builder.Configuration.GetSection("ReverseProxy")
    );

var app = builder.Build();

// permite las solicitudes desde react
app.UseCors("ReactDev");

// habilita las rutas del gateway
app.MapReverseProxy();

app.Run();