using HqCatalog.Business.Interfaces;
using HqCatalog.Business.Models;
using HqCatalog.Data.Context;
using System.Data.Entity;

namespace HqCatalog.Data.Repository
{
    public class HqRepository : IHqRepository
    {
        private readonly ApplicationDbContext _context;

        public HqRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Hq>> ObterTodos() => await _context.Hq.ToListAsync();

        public async Task<Hq> ObterPorId(int id)
        {
            return await _context.Hq.FindAsync(id);
        }

        public async Task Adicionar(Hq hq)
        {
            _context.Hq.Add(hq);
            await _context.SaveChangesAsync();
        }

        public async Task Atualizar(Hq hq)
        {
            _context.Hq.Update(hq);
            await _context.SaveChangesAsync();
        }

        public async Task Remover(int id)
        {
            var hq = await ObterPorId(id);
            if (hq != null)
            {
                _context.Hq.Remove(hq);
                await _context.SaveChangesAsync();
            }
        }
    }
}
