using Microsoft.AspNetCore.Mvc;
using HqCatalog.Business.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using HqCatalog.Business.Models.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using HqCatalog.Api.Config;

namespace HqCatalog.Mvc.Areas.Site.Controllers
{
    [Area("Site")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly JwtSettings _jwtSettings; // 🔹 Declarando corretamente

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            JwtSettings jwtSettings) // 🔹 Agora recebe o JwtSettings corretamente
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));

            // 🔹 Teste para ver se os valores do JWT estão corretos
            _logger.LogInformation($"JWT Configurado: Secret={_jwtSettings.Secret}, Expiration={_jwtSettings.ExpirationHours}");

        }



        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("Usuário logado com sucesso.");
                return RedirectToLocal(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "Login inválido.");
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var model = new ProfileViewModel
            {
                CurrentUser = user,
                UsersList = _userManager.Users.ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            user.Nome = model.CurrentUser.Nome;
            user.Email = model.CurrentUser.Email;
            user.UserName = model.CurrentUser.Email;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "Perfil atualizado com sucesso!";
                return RedirectToAction("Profile");
            }

            TempData["Error"] = "Erro ao atualizar perfil.";
            model.UsersList = _userManager.Users.ToList(); // alterado para não dar erro
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return NotFound();

            user.Nome = model.Nome;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Usuário atualizado com sucesso!";
                return RedirectToAction("Profile");
            }

            TempData["Error"] = "Erro ao atualizar usuário.";
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id, string NovaSenha, string ConfirmarSenha)
        {
            if (NovaSenha != ConfirmarSenha)
            {
                TempData["Error"] = "As senhas não coincidem.";
                return RedirectToAction("ResetPassword", new { id });
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, NovaSenha);

            if (result.Succeeded)
            {
                TempData["Success"] = "Senha redefinida com sucesso!";
                return RedirectToAction("Profile");
            }

            TempData["Error"] = "Erro ao redefinir senha.";
            return RedirectToAction("ResetPassword", new { id });
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear(); // Limpa a sessão
            HttpContext.Response.Cookies.Delete(".AspNetCore.Identity.Application"); // Remove o cookie de autenticação

            _logger.LogInformation("Usuário deslogado com sucesso.");

            return RedirectToAction("Login", "Account", new { area = "Site" });
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleUserStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            user.Ativo = !user.Ativo;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = user.Ativo ? "Usuário ativado." : "Usuário desativado.";
            return RedirectToAction("Profile");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Profile");
        }
    }
}
