using HqCatalog.Business.Interfaces;
using HqCatalog.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HqCatalog.Business.Service
{
    public class HqService : IHqService
    {
        private readonly IHqRepository _repository;

        public HqService(IHqRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Hq>> ObterTodos() => await _repository.ObterTodos();

        public async Task<Hq> ObterPorId(int id) => await _repository.ObterPorId(id);

        public async Task Adicionar(Hq hq) => await _repository.Adicionar(hq);

        public async Task Atualizar(Hq hq) => await _repository.Atualizar(hq);

        public async Task Remover(int id) => await _repository.Remover(id);
    }
}
