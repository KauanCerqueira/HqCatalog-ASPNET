using HqCatalog.Data.Context;
using HqCatalog.Business.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Configuração do banco de dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 🔹 Verifica se o banco existe e restaura caso necessário
RestaurarBancoSeNecessario(connectionString);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
});

// 🔹 Configuração do Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 🔹 Configuração do Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 🔹 Configuração da autenticação via Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Site/Account/Login";
    options.AccessDeniedPath = "/Site/Account/AcessoNegado";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // ⏳ Tempo de expiração ajustado
    options.SlidingExpiration = true; // 🔁 Renova sessão ao interagir
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// 🔹 Adiciona suporte a sessão
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 🔹 Adiciona suporte a controllers e views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 🔹 Configuração do pipeline de requisições
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// 🔹 Habilita sessão
app.UseSession();

// 🔹 Configura autenticação e autorização

app.UseAuthorization();

app.Use(async (context, next) =>
{
    if (!context.User.Identity.IsAuthenticated && !context.Request.Path.StartsWithSegments("/Site/Account/Login"))
    {
        context.Response.Redirect("/Site/Account/Login");
        return;
    }
    await next();
});

// 🔹 Configuração de rotas
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "Site" }
);

app.Run();

// 📌 Método para restaurar o banco automaticamente se necessário
void RestaurarBancoSeNecessario(string connString)
{
    try
    {
        string backupPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "BancaHQsDB.bak");
        string mdfPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "BancaHQsDB.mdf");
        string ldfPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "BancaHQsDB.ldf");

        using (var connection = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Integrated Security=True;"))
        {
            connection.Open();
            string checkDbExists = "SELECT COUNT(*) FROM sys.databases WHERE name = 'BancaHQsDB'";

            using (var command = new SqlCommand(checkDbExists, connection))
            {
                int databaseExists = (int)command.ExecuteScalar();

                if (databaseExists == 0)
                {
                    Console.WriteLine("🔹 Banco de dados não encontrado. Restaurando...");

                    string restoreSql = $@"
                        RESTORE DATABASE BancaHQsDB
                        FROM DISK = '{backupPath}'
                        WITH MOVE 'BancaHQsDB' TO '{mdfPath}',
                             MOVE 'BancaHQsDB_log' TO '{ldfPath}',
                             REPLACE;";

                    using (var restoreCommand = new SqlCommand(restoreSql, connection))
                    {
                        restoreCommand.ExecuteNonQuery();
                    }

                    Console.WriteLine("✅ Banco de dados restaurado com sucesso!");
                }
                else
                {
                    Console.WriteLine("✅ Banco de dados já existe. Nenhuma ação necessária.");
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"🚨 Erro ao restaurar o banco de dados: {ex.Message}");
    }
}
