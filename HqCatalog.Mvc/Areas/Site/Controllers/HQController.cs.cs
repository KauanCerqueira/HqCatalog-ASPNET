using HqCatalog.Data.Context;
using HqCatalog.Business.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;

[Area("Site")]
[Route("Site/Hq")]
public class HqController : Controller
{
    private readonly ApplicationDbContext _context;

    public HqController(ApplicationDbContext context)
    {
        _context = context;
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
    public IActionResult Excluir([FromBody] dynamic data)
    {
        int id = (int)data.id;

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

    // 🔹 Cadastro de HQ
    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Hq hq, IFormFile ImagemArquivo)
    {
        if (ModelState.IsValid)
        {
            if (ImagemArquivo != null && ImagemArquivo.Length > 0)
            {
                // 🔹 Salva a imagem no diretório "wwwroot/imagens/hqs"
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImagemArquivo.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens/hqs", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImagemArquivo.CopyTo(stream);
                }

                // 🔹 Define a URL da imagem salva
                hq.ImagemUrl = fileName;
            }

            _context.HQs.Add(hq);
            _context.SaveChanges();

            TempData["MensagemSucesso"] = "HQ cadastrada com sucesso!";
            return RedirectToAction("Index", "Home", new { area = "Site" });
        }

        return View(hq);
    }
}
