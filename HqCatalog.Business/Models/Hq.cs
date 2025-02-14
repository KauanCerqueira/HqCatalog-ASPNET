using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HqCatalog.Business.Models
{
    public class Hq
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string Editora { get; set; }
        public int AnoPublicacao { get; set; }
        public string Genero { get; set; }
        public string ImagemUrl { get; set; }
        public string Sinopse { get; set; }
        public virtual Prateleira Prateleira { get; set; }
        public int PrateleiraId { get; set; }
        

    }
}
