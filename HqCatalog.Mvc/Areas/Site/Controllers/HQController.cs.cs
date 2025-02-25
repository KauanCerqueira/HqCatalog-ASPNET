using HqCatalog.Data.Context;
using HqCatalog.Business.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using HqCatalog.Business.Service;

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
        if (id <= 0)
        {
            return BadRequest(new { sucesso = false, message = "ID inválido." });
        }

        var hq = _context.HQs.Find(id);
        if (hq == null)
        {
            return NotFound(new { sucesso = false, message = "HQ não encontrada." });
        }

        _context.HQs.Remove(hq);
        _context.SaveChanges();

        return Json(new { sucesso = true, message = "HQ excluída com sucesso!" });
    }

    // 🔹 Página de Cadastro
    [HttpGet("Create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("Editar")]
    [ValidateAntiForgeryToken] // 🔹 Garante segurança contra ataques CSRF
    public async Task<IActionResult> Editar(Hq model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var hqExistente = await _context.HQs.FindAsync(model.Id);
        if (hqExistente == null)
        {
            return NotFound();
        }

        // Atualiza os dados
        hqExistente.Titulo = model.Titulo;
        hqExistente.Autor = model.Autor;
        hqExistente.AnoPublicacao = model.AnoPublicacao;
        hqExistente.Editora = model.Editora;
        hqExistente.Genero = model.Genero;
        hqExistente.DescricaoCompleta = model.DescricaoCompleta;

        if (model.ImagemUrl != null)
        {
            hqExistente.ImagemUrl = model.ImagemUrl; // Atualiza a imagem caso tenha sido trocada
        }

        _context.Update(hqExistente);
        await _context.SaveChangesAsync();

        return RedirectToAction("Detalhes", new { id = model.Id }); // 🔹 Redireciona para os detalhes da HQ
    }


    [HttpPost("FiltrarHqs")]
    public IActionResult FiltrarHqs([FromBody] FiltroHqViewModel filtros)
    {
        if (filtros == null)
        {
            return BadRequest("❌ Nenhum filtro recebido.");
        }

        Console.WriteLine("🔹 Filtros Recebidos:");
        Console.WriteLine($"Editoras: {string.Join(", ", filtros.Editoras ?? new List<string>())}");
        Console.WriteLine($"Gêneros: {string.Join(", ", filtros.Generos ?? new List<string>())}");

        var hqs = _context.HQs.AsQueryable();

        if (filtros.Editoras != null && filtros.Editoras.Any())
        {
            hqs = hqs.Where(hq => filtros.Editoras.Contains(hq.Editora));
        }

        if (filtros.Generos != null && filtros.Generos.Any())
        {
            hqs = hqs.Where(hq => filtros.Generos.Contains(hq.Genero));
        }

        var resultado = hqs.Select(hq => new
        {
            id = hq.Id,
            titulo = hq.Titulo,
            editora = hq.Editora,
            editoraCor = hq.Editora.Contains("DC") ? "#1E88E5" :
                         hq.Editora.Contains("Marvel") ? "#D32F2F" : "#333",
            imagemUrl = hq.ImagemUrl,
            sinopse = hq.DescricaoCompleta
        }).ToList();

        return Json(resultado);
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] Hq hq, [FromForm] IFormFile ImagemArquivo)
    {
        _logger.LogInformation("🔹 Recebendo solicitação de cadastro de HQ...");

        try
        {
            // 📌 Salva a imagem primeiro
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
                    await ImagemArquivo.CopyToAsync(stream);
                }

                hq.ImagemUrl = fileName; // ❗️ Agora preenche antes da validação
            }
            else
            {
                _logger.LogError("❌ Nenhuma imagem foi enviada.");
                return BadRequest(new { sucesso = false, erros = new[] { new { Campo = "ImagemArquivo", Erros = new[] { "A imagem é obrigatória." } } } });
            }

            // 📌 Só valida o ModelState **depois** de preencher `ImagemUrl`
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

            _context.HQs.Add(hq);
            await _context.SaveChangesAsync();

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
