using HqCatalog.Business.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HqCatalog.Business.Interfaces
{
    public interface IHqService
    {
        Task<List<Hq>> ObterTodos();
        Task<Hq> ObterPorId(int id);
        Task Adicionar(Hq hq);
        Task Atualizar(Hq hq);
        Task Remover(int id);

        // 🔹 Novo método para salvar a imagem e retornar o nome do arquivo salvo
        Task<string> SalvarImagem(IFormFile imagemArquivo);
    }
}
