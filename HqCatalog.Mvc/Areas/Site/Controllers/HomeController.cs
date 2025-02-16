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

    }
}
