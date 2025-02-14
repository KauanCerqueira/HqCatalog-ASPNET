using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HqCatalog.Business.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; } // Armazenar senha de forma segura
        public DateTime DataCadastro { get; set; }
        public bool Ativo { get; set; }
        public string Tipo { get; set; } // Administrador ou usuário comum
        public virtual ICollection<HQFavorita> HQsFavoritas { get; set; } // Relacionamento com HQs favoritas
        public User()
        {
            HQsFavoritas = new List<HQFavorita>();
        }
    }
}
