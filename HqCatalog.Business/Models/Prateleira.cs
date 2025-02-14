using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HqCatalog.Business.Models;    

namespace HqCatalog.Business.Models
{
    public class Prateleira
    {
        public int Id { get; set; } // Identificador único
        public string Nome { get; set; } // Nome da prateleira (ex: "Super-Heróis", "Mangás", "Clássicos")
        public string Descricao { get; set; } // Descrição da prateleira
        public int CapacidadeMaxima { get; set; } // Número máximo de HQs que podem ser armazenadas
        public virtual ICollection<Hq> HQs { get; set; } // Lista de HQs associadas à prateleira

        public Prateleira()
        {
            HQs = new List<Hq>();
        }
    }
}
