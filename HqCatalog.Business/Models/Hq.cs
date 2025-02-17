using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace HqCatalog.Business.Models
{
    public class Hq
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string Editora { get; set; }
        public string DescricaoCompleta { get; set; }
        public string Personagem { get; set; }
        public int AnoPublicacao { get; set; }
        public string Genero { get; set; }
        public string ImagemUrl { get; set; }
        public string Sinopse { get; set; }

        // 🔹 Relacionamentos com outras entidades
        public virtual ICollection<Avaliacao> Avaliacoes { get; set; }
        public virtual ICollection<HQFavorita> HQsFavoritas { get; set; }

        [NotMapped] // 🔹 Isso impede que a propriedade seja mapeada no banco de dados
        public IFormFile ImagemArquivo { get; set; }

        public Hq()
        {
            Avaliacoes = new List<Avaliacao>();
            HQsFavoritas = new List<HQFavorita>();
        }
    }
}
