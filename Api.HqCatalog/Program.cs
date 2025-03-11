using HqCatalog.Data.Context;
using HqCatalog.Business.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Text;
using HqCatalog.Api.Configuration;
using Microsoft.Extensions.Options;
using HqCatalog.Business.Interfaces;
using HqCatalog.Business.Service;
using HqCatalog.Data.Repository;
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
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);

// 🔹 Obtém as configurações do JWT
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
Console.WriteLine($"🔹 JWT Secret: {jwtSettings.Secret}");
Console.WriteLine($"🔹 JWT Expiration: {jwtSettings.ExpirationHours}");
Console.WriteLine($"🔹 JWT Issuer: {jwtSettings.Issuer}");
Console.WriteLine($"🔹 JWT Audience: {jwtSettings.Audience}");

builder.Services.AddSingleton(jwtSettings);

// 🔹 Configuração da autenticação e autorização JWT
var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience
        };
    });

builder.Services.AddAuthorization(); // 🔹 Registro correto da autorização

// 🔹 Configuração do Swagger e versionamento da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerConfig();

// 🔹 Habilitar Controllers e Serviços
builder.Services.AddControllers();
builder.Services.AddScoped<IHqService, HqService>(); // 🔹 Serviço de HQ
builder.Services.AddScoped<IHqRepository, HqRepository>(); // 🔹 Repositório de HQ

#endregion

var app = builder.Build();

// 🔹 Obter o `IApiVersionDescriptionProvider` antes de iniciar a API
var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

#region 🔹 Configuração do Pipeline (Middleware)

app.UseHttpsRedirection();
app.UseAuthentication(); // 🔐 Autenticação deve vir antes da autorização
app.UseAuthorization();

// 🔹 Habilitar Swagger SEM restrição de ambiente
app.UseSwaggerConfig();

// 🔹 Mapeamento de Controllers
app.MapControllers();

// 🔹 Abrir Swagger automaticamente no navegador ao rodar a API
var swaggerUrl = "https://localhost:7295/swagger/index.html"; // 🔹 Certifique-se de que a URL está correta
Task.Delay(2000).ContinueWith(_ => Process.Start(new ProcessStartInfo
{
    FileName = swaggerUrl,
    UseShellExecute = true
}));

#endregion

app.Run();
