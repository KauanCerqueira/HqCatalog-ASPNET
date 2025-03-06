using HqCatalog.Business.Interfaces;
using HqCatalog.Business.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HqCatalog.Business.Service
{
    public class HqService : IHqService
    {
        private readonly IHqRepository _repository;
        private readonly string _uploadPath;

        public HqService(IHqRepository repository)
        {
            _repository = repository;
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens/hqs");

            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public async Task<List<Hq>> ObterTodos() => await _repository.ObterTodos();

        public async Task<Hq> ObterPorId(int id) => await _repository.ObterPorId(id);

        public async Task Adicionar(Hq hq)
        {
            if (string.IsNullOrEmpty(hq.Titulo))
                throw new Exception("O título da HQ é obrigatório.");

            await _repository.Adicionar(hq);
        }

        public async Task Atualizar(Hq hq)
        {
            if (string.IsNullOrEmpty(hq.Titulo))
                throw new Exception("O título da HQ é obrigatório.");

            await _repository.Atualizar(hq);
        }

        public async Task Remover(int id)
        {
            var hq = await _repository.ObterPorId(id);
            if (hq == null) throw new Exception("HQ não encontrada.");

            await _repository.Remover(id);
        }

        public async Task<string> SalvarImagem(IFormFile imagemArquivo)
        {
            if (imagemArquivo == null || imagemArquivo.Length == 0)
                return "placeholder.jpg"; // Se não houver imagem, usa uma padrão.

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(imagemArquivo.FileName)}";
            string filePath = Path.Combine(_uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imagemArquivo.CopyToAsync(stream);
            }

            return fileName; // Retorna apenas o nome do arquivo salvo
        }
    }
}
