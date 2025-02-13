using HqCatalog.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HqCatalog.Business.Interfaces
{
    public interface IHqRepository
    {
        Task<List<Hq>> ObterTodos();
        Task<Hq> ObterPorId(int id);
        Task Adicionar(Hq hq);
        Task Atualizar(Hq hq);
        Task Remover(int id);
    }
}
