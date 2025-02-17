using HqCatalog.Data.Context;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HqCatalog.Mvc.Areas.Site.Controllers
{
    [Area("Site")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var hqs = _context.HQs.ToList();

            ViewBag.Editoras = hqs.Select(h => h.Editora).Distinct().ToList();
            ViewBag.Generos = hqs.Select(h => h.Genero).Distinct().ToList();

            return View(hqs);
        }

        [HttpGet]
        public IActionResult SearchHqs(string query)
        {
            var hqs = _context.HQs
                .Where(h => h.Titulo.Contains(query))
                .Select(h => new
                {
                    h.Id,
                    h.Titulo,
                    h.Sinopse,
                    h.Editora,
                    h.ImagemUrl,
                    EditoraCor = h.Editora.ToLower().Contains("dc comics") ? "#1E88E5" :
                                 h.Editora.ToLower().Contains("marvel comics") ? "#D32F2F" : "#333"
                })
                .ToList();

            return Json(hqs);
        }
    }
}
