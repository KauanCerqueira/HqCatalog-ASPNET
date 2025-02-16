using System;

namespace HqCatalog.Business.Models
{
    public class Avaliacao
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; } // ID do usuário no Identity
        public int HQId { get; set; } // 🔹 Chave estrangeira para HQ
        public int Nota { get; set; }
        public string Comentario { get; set; }
        public DateTime DataCriacao { get; set; }

        // 🔹 Relacionamento com HQ
        public virtual Hq HQ { get; set; }
    }
}
