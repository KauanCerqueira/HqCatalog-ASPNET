using HqCatalog.Data.Context;
using HqCatalog.Business.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Text;
using HqCatalog.Api.Configuration;
using HqCatalog.Api.Config;

var builder = WebApplication.CreateBuilder(args);

#region 🔹 Configuração de Serviços

// 🔹 Configuração do banco de dados
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Configurar Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 🔹 Configuração do JWT
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Secret"]);
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// 🔹 Configuração do Swagger e Versionamento da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
});
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("Jwt").Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerConfig();

// 🔹 Habilitar Controllers
builder.Services.AddControllers();

#endregion

var app = builder.Build();

// 🔹 Obter o `IApiVersionDescriptionProvider` antes de iniciar a API
var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

#region 🔹 Configuração do Pipeline (Middleware)

// 🔹 Middleware de segurança
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

// 🔹 Habilitar Swagger SEM restrição de ambiente
app.UseSwaggerConfig();

// 🔹 Mapeamento de Controllers
app.MapControllers();

// 🔹 Abrir Swagger automaticamente no navegador ao rodar a API
var swaggerUrl = "https://localhost:7295/swagger";
Task.Delay(2000).ContinueWith(_ => Process.Start(new ProcessStartInfo
{
    FileName = swaggerUrl,
    UseShellExecute = true
}));

#endregion

app.Run();
