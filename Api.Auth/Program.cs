using Api.Auth;
using Api.Auth.Repository;
using Api.Auth.Security;
using Api.Auth.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// agrega soporte para controllers
builder.Services.AddControllers();

// agrega swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// registra la fabrica de conexiones como singleton
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

// registra los repositorios
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<BitacoraRepository>();

// registra los servicios
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PasswordService>();

// registra el servicio que genera tokens
builder.Services.AddSingleton<ITokenService, TokenService>();

// obtiene la configuracion de jwt
string jwtKey =
    builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "No se encontro la clave JWT."
    );

string jwtIssuer =
    builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "No se encontro el issuer JWT."
    );

string jwtAudience =
    builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "No se encontro el audience JWT."
    );

// configura la autenticacion con jwt
builder.Services
    .AddAuthentication(
        options =>
        {
            options.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;

            options.DefaultChallengeScheme =
                JwtBearerDefaults.AuthenticationScheme;
        }
    )
    .AddJwtBearer(
        options =>
        {
            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,

                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtKey)
                        ),

                    ClockSkew = TimeSpan.Zero
                };
        }
    );

// agrega autorizacion
builder.Services.AddAuthorization();

// permite llamadas desde react durante desarrollo
builder.Services.AddCors(
    options =>
    {
        options.AddPolicy(
            "React",
            policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            }
        );
    }
);

var app = builder.Build();

// habilita swagger durante desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// permite las solicitudes de react
app.UseCors("React");

// habilita autenticacion
app.UseAuthentication();

// habilita autorizacion
app.UseAuthorization();

// mapea los controllers
app.MapControllers();

// registra los endpoints de autenticacion
app.MapAuthEndpoints();

app.Run();