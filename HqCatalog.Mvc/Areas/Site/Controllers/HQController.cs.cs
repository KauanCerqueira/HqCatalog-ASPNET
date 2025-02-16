using HqCatalog.Data.Context;
using HqCatalog.Business.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HqCatalog.Areas.Site.Controllers
{
    [Area("Site")] // 🔹 Define a área corretamente
    public class HqController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HqController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Detalhes(int id)
        {
            var hq = _context.HQs.FirstOrDefault(h => h.Id == id);
            if (hq == null)
            {
                return NotFound();
            }

            return View(hq);
        }
    }
}
