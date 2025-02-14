namespace HqCatalog.Mvc.Controllers
{
    using global::HqCatalog.Business.Models;
    using global::HqCatalog.Data.Context;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;

    namespace HqCatalog.Mvc.Controllers
    {
        public class HQController : Controller
        {
            private readonly ApplicationDbContext _context;

            public HQController(ApplicationDbContext context)
            {
                _context = context;
            }

            // 🔹 Listar todas as HQs
            public async Task<IActionResult> Index()
            {
                var hqs = await _context.HQs.Include(h => h.Prateleira).ToListAsync();
                return View(hqs);
            }

            // 🔹 Criar uma HQ (GET)
            public IActionResult Create()
            {
                return View();
            }

            // 🔹 Criar uma HQ (POST)
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(Hq hq)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(hq);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(hq);
            }
        }
    }

}
