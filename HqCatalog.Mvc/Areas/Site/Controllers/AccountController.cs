using Microsoft.AspNetCore.Mvc;
using HqCatalog.Business.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;

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

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
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
        public async Task<IActionResult> Register(string nome, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                TempData["Error"] = "Preencha todos os campos!";
                return View();
            }

            if (password != confirmPassword)
            {
                TempData["Error"] = "As senhas não coincidem!";
                return View();
            }

            var user = new ApplicationUser
            {
                Nome = nome,
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home", new { area = "Site" });
            }

            TempData["Error"] = result.Errors.FirstOrDefault()?.Description ?? "Erro ao registrar usuário.";
            return View();
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
        public async Task<IActionResult> Profile(ApplicationUser model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Usuário solicitou logout...");

            // 🔹 Desloga o usuário do Identity
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            // 🔹 Remove manualmente os cookies de autenticação
            HttpContext.Response.Cookies.Delete(".AspNetCore.Identity.Application");
            HttpContext.Response.Cookies.Delete("Identity.External");

            // 🔹 Limpa qualquer informação do usuário na sessão
            HttpContext.Session.Clear();
            HttpContext.User = new System.Security.Claims.ClaimsPrincipal();

            _logger.LogInformation("Usuário deslogado com sucesso!");

            return RedirectToAction("Login", "Account", new { area = "Site" });
        }

    }
}
