using HqCatalog.Api.Models;
using HqCatalog.Business.Interfaces;
using HqCatalog.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HqCatalog.Api.Controllers
{
    [Route("api/hqs")]
    [ApiController]
    [Authorize] // 🔐 Agora todos os métodos exigem autenticação por padrão
    public class HqController : ControllerBase // 🔹 Alterado para ControllerBase
    {
        private readonly IHqService _hqService;

        public HqController(IHqService hqService) // 🔹 Removida a dependência de notificador
        {
            _hqService = hqService ?? throw new ArgumentNullException(nameof(hqService));
            _hqService = hqService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Obtém todas as HQs", Description = "Retorna uma lista de HQs disponíveis no catálogo.")]
        public async Task<IEnumerable<Hq>> ObterTodos()
        {
            return await _hqService.ObterTodos();
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Obtém uma HQ específica", Description = "Busca uma HQ pelo seu ID.")]
        public async Task<ActionResult<Hq>> ObterPorId(int id)
        {
            var hq = await _hqService.ObterPorId(id);
            if (hq == null) return NotFound();

            return hq;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Adiciona uma nova HQ", Description = "Insere uma nova HQ no catálogo.")]
        public async Task<ActionResult<Hq>> Adicionar([FromBody] HqCreateDTO dto)
        {
            try
            {
                var novaHq = new Hq
                {
                    Titulo = dto.Titulo,
                    Autor = dto.Autor,
                    Editora = dto.Editora,
                    DescricaoCompleta = dto.DescricaoCompleta,
                    Personagem = dto.Personagem,
                    AnoPublicacao = dto.AnoPublicacao,
                    Genero = dto.Genero,
                    ImagemUrl = dto.ImagemUrl = "NULL",
                    Sinopse = dto.Sinopse
                };

                await _hqService.Adicionar(novaHq);
                return CreatedAtAction(nameof(ObterPorId), new { id = novaHq.Id }, novaHq);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Atualiza uma HQ existente", Description = "Atualiza os dados de uma HQ pelo seu ID.")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] HqAtualizarDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { sucesso = false, erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
            }

            var hqExistente = await _hqService.ObterPorId(id);
            if (hqExistente == null)
            {
                return NotFound();
            }

            // Atualizando os campos necessários
            hqExistente.Titulo = model.Titulo;
            hqExistente.Autor = model.Autor;
            hqExistente.AnoPublicacao = model.AnoPublicacao;
            hqExistente.Editora = model.Editora;
            hqExistente.Genero = model.Genero;
            hqExistente.DescricaoCompleta = model.DescricaoCompleta;
            hqExistente.Personagem = model.Personagem;
            hqExistente.Sinopse = model.Sinopse;

            await _hqService.Atualizar(hqExistente);

            return Ok(new { sucesso = true, mensagem = "HQ atualizada com sucesso.", redirectUrl = Url.Action("Detalhes", new { id }) });
        }


        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Remove uma HQ", Description = "Exclui uma HQ do catálogo pelo ID.")]
        public async Task<ActionResult> Remover(int id)
        {
            try
            {
                await _hqService.Remover(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("upload-imagem/{id:int}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Envia uma imagem de HQ e associa ao ID", Description = "Faz o upload de uma imagem para a HQ e atualiza a URL no banco de dados.")]
        public async Task<ActionResult> UploadImagem(int id, IFormFile imagem)
        {
            try
            {
                // ✅ Valida se a imagem foi enviada
                if (imagem == null || imagem.Length == 0)
                    return BadRequest(new { success = false, message = "A imagem é obrigatória." });

                // ✅ Busca a HQ pelo ID
                var hq = await _hqService.ObterPorId(id);
                if (hq == null) return NotFound(new { success = false, message = "HQ não encontrada." });

                // ✅ Salva a imagem na pasta desejada
                var nomeArquivo = await _hqService.SalvarImagem(imagem);

                // ✅ Atualiza a URL da imagem na HQ
                hq.ImagemUrl = nomeArquivo;
                await _hqService.Atualizar(hq);

                return Ok(new { success = true, fileName = nomeArquivo });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
