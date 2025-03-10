using HqCatalog.Data.Context;
using HqCatalog.Business.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

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

    // 🔹 Permite que qualquer usuário veja os detalhes de uma HQ
    [AllowAnonymous]
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

    // 🔹 Permite que qualquer usuário filtre as HQs sem precisar de login
    [AllowAnonymous]
    [HttpPost("FiltrarHqs")]
    public IActionResult FiltrarHqs([FromBody] FiltroHqViewModel filtros)
    {
        if (filtros == null)
        {
            return BadRequest("❌ Nenhum filtro recebido.");
        }

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

    // 🔹 Apenas administradores podem criar HQs
    [Authorize(Roles = "Admin")]
    [HttpGet("Create")]
    public IActionResult Create()
    {
        return View();
    }

    [Area("Site")]
    [Authorize(Roles = "Admin")]
    [HttpGet("Site/Hq/Editar/{id}")]
    public async Task<IActionResult> Editar(int id)
    {
        var hq = await _context.HQs.FindAsync(id);

        if (hq == null)
            return NotFound();

        return View(hq);
    }

    [Area("Site")]
    [Authorize(Roles = "Admin")]
    [HttpPost("Site/Hq/Editar/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, [FromForm] Hq model)
    {
        if (id != model.Id)
        {
            return BadRequest("ID não corresponde.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new { sucesso = false, erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
        }

        var hqExistente = await _context.HQs.FindAsync(id);
        if (hqExistente == null)
        {
            return NotFound();
        }

        hqExistente.Titulo = model.Titulo;
        hqExistente.Autor = model.Autor;
        hqExistente.AnoPublicacao = model.AnoPublicacao;
        hqExistente.Editora = model.Editora;
        hqExistente.Genero = model.Genero;
        hqExistente.DescricaoCompleta = model.DescricaoCompleta;

        await _context.SaveChangesAsync();

        return Json(new { sucesso = true, redirectUrl = Url.Action("Detalhes", new { id = model.Id }) });
    }


    // 🔹 Apenas administradores podem excluir HQs
    [Authorize(Roles = "Admin")]
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

    // 🔹 Apenas administradores podem criar novas HQs
    [Authorize(Roles = "Admin")]
    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] Hq hq, [FromForm] IFormFile ImagemArquivo)
    {
        _logger.LogInformation("🔹 Recebendo solicitação de cadastro de HQ...");

        try
        {
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

                hq.ImagemUrl = fileName;
            }
            else
            {
                _logger.LogError("❌ Nenhuma imagem foi enviada.");
                return BadRequest(new { sucesso = false, erros = new[] { new { Campo = "ImagemArquivo", Erros = new[] { "A imagem é obrigatória." } } } });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { sucesso = false, erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
            }

            _context.HQs.Add(hq);
            await _context.SaveChangesAsync();

            _logger.LogInformation("✅ HQ cadastrada com sucesso: {Titulo} (ID: {Id})", hq.Titulo, hq.Id);

            return Json(new
            {
                sucesso = true,
                imagemUrl = $"~/Images/HqImage/{hq.ImagemUrl}",
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
