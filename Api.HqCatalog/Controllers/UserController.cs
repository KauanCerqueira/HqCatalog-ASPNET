using HqCatalog.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HqCatalog.Api.Controllers
{
    [Route("api/usuarios")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Obtém todos os usuários cadastrados.
        /// </summary>
        [HttpGet]
        
        [SwaggerOperation(Summary = "Lista todos os usuários", Description = "Retorna uma lista de todos os usuários cadastrados.")]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> ObterTodosUsuarios()
        {
            var usuarios = _userManager.Users.ToList();
            return Ok(usuarios);
        }

        /// <summary>
        /// Obtém um usuário pelo ID.
        /// </summary>
        [HttpGet("{id}")]
        
        [SwaggerOperation(Summary = "Busca um usuário por ID", Description = "Retorna os detalhes de um usuário específico.")]
        public async Task<ActionResult<ApplicationUser>> ObterUsuarioPorId(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
                return NotFound(new { sucesso = false, message = "Usuário não encontrado." });

            return Ok(usuario);
        }

        /// <summary>
        /// Atualiza os dados do usuário (Nome, Email).
        /// </summary>
        [HttpPut("{id}")]
        
        [SwaggerOperation(Summary = "Atualiza os dados do usuário", Description = "Permite atualizar nome e email do usuário.")]
        public async Task<IActionResult> AtualizarUsuario(string id, [FromBody] AtualizarUsuarioDTO model)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
                return NotFound(new { sucesso = false, message = "Usuário não encontrado." });

            usuario.Nome = model.Nome;
            usuario.Email = model.Email;
            usuario.UserName = model.Email;

            var resultado = await _userManager.UpdateAsync(usuario);
            if (!resultado.Succeeded)
                return BadRequest(new { sucesso = false, erros = resultado.Errors });

            return Ok(new { sucesso = true, message = "Usuário atualizado com sucesso!" });
        }

        /// <summary>
        /// Altera a senha do usuário.
        /// </summary>
        [HttpPut("alterar-senha/{id}")]
        
        [SwaggerOperation(Summary = "Altera a senha do usuário", Description = "Permite alterar a senha do usuário.")]
        public async Task<IActionResult> AlterarSenha(string id, [FromBody] AlterarSenhaDTO model)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
                return NotFound(new { sucesso = false, message = "Usuário não encontrado." });

            var resultado = await _userManager.ChangePasswordAsync(usuario, model.SenhaAtual, model.NovaSenha);
            if (!resultado.Succeeded)
                return BadRequest(new { sucesso = false, erros = resultado.Errors });

            return Ok(new { sucesso = true, message = "Senha alterada com sucesso!" });
        }

        /// <summary>
        /// Atribui um papel (role) a um usuário.
        /// </summary>
        [HttpPost("atribuir-role")]
        
        [SwaggerOperation(Summary = "Atribui um papel ao usuário", Description = "Permite adicionar uma role ao usuário.")]
        public async Task<IActionResult> AtribuirRole([FromBody] AtribuirRoleDTO model)
        {
            var usuario = await _userManager.FindByIdAsync(model.UsuarioId);
            if (usuario == null)
                return NotFound(new { sucesso = false, message = "Usuário não encontrado." });

            if (!await _roleManager.RoleExistsAsync(model.Role))
                return BadRequest(new { sucesso = false, message = "Role informada não existe." });

            var resultado = await _userManager.AddToRoleAsync(usuario, model.Role);
            if (!resultado.Succeeded)
                return BadRequest(new { sucesso = false, erros = resultado.Errors });

            return Ok(new { sucesso = true, message = $"Papel {model.Role} atribuído com sucesso ao usuário!" });
        }
    }

    // DTOs auxiliares
    public class AtualizarUsuarioDTO
    {
        public string Nome { get; set; }
        public string Email { get; set; }
    }

    public class AlterarSenhaDTO
    {
        public string SenhaAtual { get; set; }
        public string NovaSenha { get; set; }
    }

    public class AtribuirRoleDTO
    {
        public string UsuarioId { get; set; }
        public string Role { get; set; }
    }
}
