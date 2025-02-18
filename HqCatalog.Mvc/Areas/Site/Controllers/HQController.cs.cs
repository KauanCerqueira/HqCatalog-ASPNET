using HqCatalog.Data.Context;
using HqCatalog.Business.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;

[Area("Site")]
[Route("Site/Hq")]
public class HqController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HqController> _logger;

    public HqController(ApplicationDbContext context, ILogger<HqController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("Detalhes/{id}")]
    public IActionResult Detalhes(int id)
    {
        var hq = _context.HQs.FirstOrDefault(h => h.Id == id);
        if (hq == null)
        {
            return NotFound();
        }
        return View(hq);
    }

    [HttpPost("Excluir")]
    public IActionResult Excluir([FromBody] int id)
    {
        var hq = _context.HQs.FirstOrDefault(h => h.Id == id);
        if (hq == null)
        {
            return NotFound(new { message = "HQ não encontrada." });
        }

        _context.HQs.Remove(hq);
        _context.SaveChanges();

        return Ok(new { message = "HQ excluída com sucesso!" });
    }

    // 🔹 Página de Cadastro
    [HttpGet("Create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public IActionResult Create([FromForm] Hq hq, [FromForm] IFormFile ImagemArquivo)
    {
        _logger.LogInformation("🔹 Recebendo solicitação de cadastro de HQ...");

        // Verifica se a imagem foi enviada corretamente
        if (ImagemArquivo == null || ImagemArquivo.Length == 0)
        {
            _logger.LogError("❌ Nenhuma imagem foi enviada.");
            return BadRequest(new { sucesso = false, erros = new[] { new { Campo = "ImagemArquivo", Erros = new[] { "A imagem é obrigatória." } } } });
        }

        _logger.LogInformation("📌 Imagem recebida: {NomeArquivo}", ImagemArquivo.FileName);

        // Verifica se o ModelState é válido
        if (!ModelState.IsValid)
        {
            var erros = ModelState
                .Where(x => x.Value.Errors.Any())
                .Select(x => new
                {
                    Campo = x.Key ?? "Desconhecido",
                    Erros = x.Value.Errors?.Select(e => e.ErrorMessage).ToList() ?? new List<string> { "Erro desconhecido" }
                })
                .ToList();

            _logger.LogError("❌ Erros de validação: {Erros}", erros);
            return BadRequest(new { sucesso = false, erros });
        }

        try
        {
            // Diretório de armazenamento das imagens
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens/hqs");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Gera um nome único para a imagem
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImagemArquivo.FileName)}";
            string filePath = Path.Combine(uploadPath, fileName);

            // Salva a imagem no diretório
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                ImagemArquivo.CopyTo(stream);
            }

            hq.ImagemUrl = fileName;

            _context.HQs.Add(hq);
            _context.SaveChanges();

            _logger.LogInformation("✅ HQ cadastrada com sucesso: {Titulo} (ID: {Id})", hq.Titulo, hq.Id);

            return Json(new
            {
                sucesso = true,
                imagemUrl = $"/imagens/hqs/{hq.ImagemUrl}",
                editora = hq.Editora
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("🚨 Erro interno ao cadastrar HQ: {Mensagem}", ex.Message);
            return StatusCode(500, new { sucesso = false, erro = "Erro interno no servidor. Tente novamente mais tarde." });
        }
    }
}
