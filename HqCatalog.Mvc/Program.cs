using HqCatalog.Data.Context;
using HqCatalog.Business.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Configuração do banco de dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

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

    options.ExpireTimeSpan = TimeSpan.FromMinutes(5); // 🔹 Sessão expira em 5 minutos
    options.SlidingExpiration = false; // 🔹 Evita que a sessão seja renovada automaticamente
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // 🔹 Obrigatório para HTTPS
    options.Cookie.SameSite = SameSiteMode.None; // 🔹 Necessário para funcionar corretamente em cross-site

    // 🔹 Garante que o cookie seja removido corretamente no logout
    options.Events.OnSigningOut = async context =>
    {
        await Task.CompletedTask;
    };
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
    defaults: new { area = "Site" } // 🔹 Mantido conforme sua solicitação
);

app.Run();
