namespace HqCatalog.Api.Models
{
    public class HqCreateDTO
    {
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string Editora { get; set; }
        public string DescricaoCompleta { get; set; }
        public string Personagem { get; set; }
        public int AnoPublicacao { get; set; }
        public string Genero { get; set; }
        public string ImagemUrl { get; set; }
        public string Sinopse { get; set; }
    }
}
