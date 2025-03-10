using HqCatalog.Api.Config;
using HqCatalog.Api.Models;
using HqCatalog.Business.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HqCatalog.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;

        public AuthController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    JwtSettings jwtSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));

            // 🔹 Depuração: Mostra o valor de `_jwtSettings.Secret`
            Console.WriteLine($"🔹 JWT Secret: {_jwtSettings.Secret}");
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] ApplicationUserRegisterDTO model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                Nome = model.Nome,
                DataCadastro = DateTime.UtcNow,
                Ativo = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { success = false, message = "Erro ao criar usuário", errors = result.Errors });
            }

            var token = GenerateJwtToken(user);
            return Ok(new { success = true, token });
        }

        /// <summary>
        /// Autentica um usuário e retorna um token JWT.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized(new { success = false, message = "Usuário ou senha inválidos" });

            var token = GenerateJwtToken(user);
            return Ok(new { success = true, token });
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret); // 🔹 Aqui usamos _jwtSettings

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Tipo.ToString()) // 🔹 Convertendo Enum para string
            }),
                Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Modelo para registro de usuário.
        /// </summary>
        public class RegisterModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        /// <summary>
        /// Modelo para login de usuário.
        /// </summary>
        public class LoginModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        /// <summary>
        /// Modelo para armazenar usuários temporariamente (substitua por um banco de dados real).
        /// </summary>
        public class UserModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
        }
    }
}
