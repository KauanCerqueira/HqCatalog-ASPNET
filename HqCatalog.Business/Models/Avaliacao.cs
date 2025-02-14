using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HqCatalog.Business.Models
{
    public class Avaliacao
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int HQId { get; set; }
        public int Nota { get; set; } // De 1 a 10, por exemplo
        public string Comentario { get; set; }
        public DateTime DataCriacao { get; set; }
        public virtual User Usuario { get; set; }
        public virtual Hq HQ { get; set; }
    }
}
