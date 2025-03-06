using HqCatalog.Business.Interfaces;
using HqCatalog.Business.Models;
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
    public class HqController : MainController
    {
        private readonly IHqService _hqService;

        public HqController(INotificador notificador, IHqService hqService) : base(notificador)
        {
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
        [SwaggerOperation(Summary = "Adiciona uma nova HQ", Description = "Insere uma nova HQ no catálogo.")]
        public async Task<ActionResult<Hq>> Adicionar(Hq hq)
        {
            try
            {
                await _hqService.Adicionar(hq);
                return CreatedAtAction(nameof(ObterPorId), new { id = hq.Id }, hq);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualiza uma HQ existente", Description = "Atualiza os dados de uma HQ pelo seu ID.")]
        public async Task<ActionResult> Atualizar(int id, Hq hq)
        {
            if (id != hq.Id) return BadRequest("IDs não correspondem.");
            try
            {
                await _hqService.Atualizar(hq);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
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

        [HttpPost("upload-imagem")]
        [SwaggerOperation(Summary = "Envia uma imagem de HQ", Description = "Faz o upload de uma imagem para a HQ.")]
        public async Task<ActionResult> UploadImagem(IFormFile imagem)
        {
            try
            {
                var nomeArquivo = await _hqService.SalvarImagem(imagem);
                return Ok(new { success = true, fileName = nomeArquivo });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
