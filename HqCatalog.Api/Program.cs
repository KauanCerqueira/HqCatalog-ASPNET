using HqCatalog.Api.Data;
using HqCatalog.Business.Interfaces;
using HqCatalog.Business.Service;
using HqCatalog.Business.Services;
using HqCatalog.Data.Context;
using HqCatalog.Data.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar banco de dados
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar Repositórios e Serviços
builder.Services.AddScoped<IHqRepository, HqRepository>();
builder.Services.AddScoped<IHqService, HqService>();

// Adicionar serviços MVC e API
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
