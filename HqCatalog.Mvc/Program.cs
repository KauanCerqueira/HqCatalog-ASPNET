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

// 🔹 Configuração do banco no Entity Framework
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
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.SlidingExpiration = false;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Events.OnSigningOut = async context => { await Task.CompletedTask; };
});

// 🔹 Adiciona suporte a sessão
builder.Services.AddSession();

// 🔹 Adiciona suporte a controllers e views
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// 🔹 Habilita sessão
app.UseSession();

// 🔹 Configura autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// 🔹 Rota para páginas dentro da pasta "Areas"
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

// 🔹 Rota padrão para direcionar a área "Site" por padrão
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
