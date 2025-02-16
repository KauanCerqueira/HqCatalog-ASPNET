using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace HqCatalog.Business.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nome { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool Ativo { get; set; }
        public UserRole Tipo { get; set; } // Agora usando Enum

        public virtual ICollection<HQFavorita> HQsFavoritas { get; set; }

        public ApplicationUser()
        {
            HQsFavoritas = new List<HQFavorita>();
            DataCadastro = DateTime.UtcNow;
            Ativo = true;
        }
    }

    public enum UserRole
    {
        Usuario = 0,
        Administrador = 1
    }
}
