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
    public IActionResult Create(Hq hq, IFormFile ImagemArquivo)
    {
        _logger.LogInformation("🔹 Recebendo solicitação de cadastro de HQ...");

        // 🔹 Log dos dados recebidos
        _logger.LogInformation("📌 Dados Recebidos:");
        _logger.LogInformation("➡ Titulo: {Titulo}", hq.Titulo ?? "(Vazio)");
        _logger.LogInformation("➡ Autor: {Autor}", hq.Autor ?? "(Vazio)");
        _logger.LogInformation("➡ Editora: {Editora}", hq.Editora ?? "(Vazio)");
        _logger.LogInformation("➡ AnoPublicacao: {AnoPublicacao}", hq.AnoPublicacao);
        _logger.LogInformation("➡ Genero: {Genero}", hq.Genero ?? "(Vazio)");
        _logger.LogInformation("➡ Personagem: {Personagem}", hq.Personagem ?? "(Vazio)");
        _logger.LogInformation("➡ Sinopse: {DescricaoCompleta}", hq.DescricaoCompleta ?? "(Vazio)");
        _logger.LogInformation("➡ ImagemArquivo: {ImagemArquivo}", ImagemArquivo?.FileName ?? "Nenhum arquivo enviado");

        if (!ModelState.IsValid)
        {
            var erros = ModelState
                .Where(x => x.Value.Errors.Any())
                .Select(x => new
                {
                    Campo = x.Key,
                    Erros = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                })
                .ToList();

            if (!erros.Any())
            {
                erros.Add(new { Campo = "Desconhecido", Erros = new List<string> { "Erro desconhecido no envio." } });
            }

            foreach (var erro in erros)
            {
                _logger.LogError("❌ Campo: {Campo}, Erros: {Erros}", erro.Campo, string.Join(", ", erro.Erros));
            }

            return Json(new { sucesso = false, erros });
        }

        if (ImagemArquivo != null && ImagemArquivo.Length > 0)
        {
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens/hqs");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImagemArquivo.FileName)}";
            string filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                ImagemArquivo.CopyTo(stream);
            }

            hq.ImagemUrl = fileName;
        }
        else
        {
            hq.ImagemUrl = "placeholder.jpg";
        }

        _context.HQs.Add(hq);
        _context.SaveChanges();

        _logger.LogInformation("✅ HQ cadastrada com sucesso: {Titulo} (ID: {Id})", hq.Titulo, hq.Id);

        return Json(new { sucesso = true });
    }
}
