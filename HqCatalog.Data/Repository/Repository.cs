using HqCatalog.Business.Interfaces;
using HqCatalog.Business.Models;
using HqCatalog.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HqCatalog.Data.Repository
{
    public class HqRepository : IHqRepository
    {
        private readonly ApplicationDbContext _context;

        public HqRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🔹 Obtém todas as HQs incluindo a prateleira relacionada
        public async Task<List<Hq>> ObterTodos()
        {
            return await _context.HQs
                .Include(h => h.Prateleira) // Para carregar os dados da Prateleira junto
                .ToListAsync();
        }

        // 🔹 Obtém uma HQ específica pelo ID, incluindo a prateleira
        public async Task<Hq> ObterPorId(int id)
        {
            return await _context.HQs
                .Include(h => h.Prateleira)
                .FirstOrDefaultAsync(h => h.Id == id); // Evita erro ao buscar
        }

        // 🔹 Adiciona uma nova HQ ao banco de dados
        public async Task Adicionar(Hq hq)
        {
            await _context.HQs.AddAsync(hq);
            await _context.SaveChangesAsync();
        }

        // 🔹 Atualiza uma HQ existente no banco
        public async Task Atualizar(Hq hq)
        {
            _context.HQs.Update(hq);
            await _context.SaveChangesAsync();
        }

        // 🔹 Remove uma HQ do banco de dados
        public async Task Remover(int id)
        {
            var hq = await ObterPorId(id);
            if (hq != null)
            {
                _context.HQs.Remove(hq);
                await _context.SaveChangesAsync();
            }
        }
    }
}
