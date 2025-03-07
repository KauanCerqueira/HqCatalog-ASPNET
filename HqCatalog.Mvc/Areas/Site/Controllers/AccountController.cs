using Microsoft.AspNetCore.Mvc;
using HqCatalog.Business.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using System;

namespace HqCatalog.Mvc.Areas.Site.Controllers
{
    [Area("Site")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Preencha todos os campos!";
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                TempData["Error"] = "Email ou senha incorretos!";
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home", new { area = "Site" });
            }

            TempData["Error"] = "Email ou senha incorretos!";
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string nome, string email, string password, string confirmPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
                {
                    return Json(new { success = false, error = "Preencha todos os campos!" });
                }

                if (password != confirmPassword)
                {
                    return Json(new { success = false, error = "As senhas não coincidem!" });
                }

                var user = new ApplicationUser
                {
                    Nome = nome,
                    UserName = email,
                    Email = email,
                    DataCadastro = DateTime.UtcNow,
                    Ativo = true,
                    Tipo = UserRole.Usuario // Define o tipo de usuário padrão
                };

                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    // 🔹 Loga o usuário automaticamente após registro
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    _logger.LogInformation("✅ Usuário registrado e logado: {Email}", email);
                    return Json(new { success = true });
                }

                var errorMessage = result.Errors.FirstOrDefault()?.Description ?? "Erro desconhecido ao registrar usuário.";
                _logger.LogError("❌ Erro ao registrar usuário {Email}: {Erro}", email, errorMessage);
                return Json(new { success = false, error = errorMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError("❌ Erro inesperado ao registrar usuário: {Mensagem}", ex.Message);
                return Json(new { success = false, error = "Erro interno no servidor. Tente novamente mais tarde." });
            }
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(user);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ApplicationUser model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.Id != model.Id)
            {
                return Unauthorized();
            }

            user.Nome = model.Nome;
            user.Email = model.Email;
            user.UserName = model.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                TempData["Error"] = "Erro ao atualizar perfil.";
                return View(user);
            }

            TempData["Success"] = "Perfil atualizado com sucesso!";
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // 🔹 Mantém a segurança contra CSRF
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("🔹 Logout chamado via POST!");

            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            HttpContext.Response.Cookies.Delete(".AspNetCore.Identity.Application");
            HttpContext.Session.Clear();
            HttpContext.User = new System.Security.Claims.ClaimsPrincipal();

            _logger.LogInformation("✅ Logout finalizado com sucesso!");

            return RedirectToAction("Login", "Account", new { area = "Site" });
        }

    }
}
